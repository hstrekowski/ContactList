using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Contracts.Security;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentValidation.Results;
using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.CreateContact
{
    /// <summary>
    /// Persists a new <see cref="Contact"/>. Performs the cross-aggregate checks that
    /// FluentValidation cannot do on its own:
    /// <list type="bullet">
    /// <item>Email uniqueness — looked up via <see cref="IContactRepository.ExistsByEmailAsync"/>.</item>
    /// <item>Category existence and the category-specific subcategory rules
    ///       (Służbowy → existing subcategory required, Inny → custom name required,
    ///       Prywatny → no subcategory).</item>
    /// </list>
    /// </summary>
    public sealed class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Guid>
    {
        private const string BusinessCategoryName = "Służbowy";
        private const string PrivateCategoryName = "Prywatny";
        private const string OtherCategoryName = "Inny";

        private readonly IContactRepository _contactRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public CreateContactCommandHandler(
            IContactRepository contactRepository,
            ICategoryRepository categoryRepository,
            ISubcategoryRepository subcategoryRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _categoryRepository = categoryRepository;
            _subcategoryRepository = subcategoryRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var email = new Email(request.Email);
            var phoneNumber = new PhoneNumber(request.PhoneNumber);
            var password = new Password(request.Password);

            if (await _contactRepository.ExistsByEmailAsync(email, excludeContactId: null, cancellationToken))
                throw new ConflictException($"A contact with email '{email.Value}' already exists.");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.CategoryId);

            var subcategoryId = await ResolveSubcategoryAsync(category, request, cancellationToken);

            var passwordHash = _passwordHasher.Hash(password.Value);

            var contact = new Contact(
                request.FirstName,
                request.LastName,
                email,
                passwordHash,
                phoneNumber,
                request.DateOfBirth,
                category.Id,
                subcategoryId);

            await _contactRepository.AddAsync(contact, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return contact.Id;
        }

        private async Task<Guid?> ResolveSubcategoryAsync(
            Category category,
            CreateContactCommand request,
            CancellationToken cancellationToken)
        {
            switch (category.Name)
            {
                case BusinessCategoryName:
                    if (request.SubcategoryId is null)
                        throw BuildValidationException(
                            nameof(request.SubcategoryId),
                            "Subcategory is required for category 'Służbowy'.");

                    if (!string.IsNullOrWhiteSpace(request.SubcategoryName))
                        throw BuildValidationException(
                            nameof(request.SubcategoryName),
                            "Subcategory name must be empty for category 'Służbowy' — pick one from the dictionary.");

                    var belongs = await _subcategoryRepository.ExistsInCategoryAsync(
                        request.SubcategoryId.Value, category.Id, cancellationToken);

                    if (!belongs)
                        throw new NotFoundException(
                            $"Subcategory '{request.SubcategoryId.Value}' does not exist in category '{category.Name}'.");

                    return request.SubcategoryId;

                case OtherCategoryName:
                    if (request.SubcategoryId is not null)
                        throw BuildValidationException(
                            nameof(request.SubcategoryId),
                            "Subcategory id must be empty for category 'Inny' — provide a custom name instead.");

                    if (string.IsNullOrWhiteSpace(request.SubcategoryName))
                        throw BuildValidationException(
                            nameof(request.SubcategoryName),
                            "Subcategory name is required for category 'Inny'.");

                    var newSubcategory = new Subcategory(request.SubcategoryName, category.Id);
                    await _subcategoryRepository.AddAsync(newSubcategory, cancellationToken);
                    return newSubcategory.Id;

                case PrivateCategoryName:
                    if (request.SubcategoryId is not null)
                        throw BuildValidationException(
                            nameof(request.SubcategoryId),
                            "Category 'Prywatny' does not allow a subcategory.");

                    if (!string.IsNullOrWhiteSpace(request.SubcategoryName))
                        throw BuildValidationException(
                            nameof(request.SubcategoryName),
                            "Category 'Prywatny' does not allow a subcategory.");

                    return null;

                default:
                    throw BuildValidationException(
                        nameof(request.CategoryId),
                        $"Unsupported category '{category.Name}'.");
            }
        }

        private static ValidationException BuildValidationException(string property, string message) =>
            new(new[] { new ValidationFailure(property, message) });
    }
}
