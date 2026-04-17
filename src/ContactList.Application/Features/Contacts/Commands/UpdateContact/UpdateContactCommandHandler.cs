using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Application.Contracts.Security;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using FluentValidation.Results;
using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.UpdateContact
{
    /// <summary>
    /// Updates an existing <see cref="Contact"/>. Re-runs the same cross-aggregate
    /// checks as <c>CreateContactCommandHandler</c>: email uniqueness (excluding the
    /// row being edited), category existence, and the category-specific subcategory
    /// rules. The password hash is only rewritten when the command carries a new value.
    /// </summary>
    public sealed class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand>
    {
        private const string BusinessCategoryName = "Służbowy";
        private const string PrivateCategoryName = "Prywatny";
        private const string OtherCategoryName = "Inny";

        private readonly IContactRepository _contactRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateContactCommandHandler(
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

        public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Contact), request.Id);

            var email = new Email(request.Email);
            var phoneNumber = new PhoneNumber(request.PhoneNumber);

            if (await _contactRepository.ExistsByEmailAsync(email, excludeContactId: contact.Id, cancellationToken))
                throw new ConflictException($"A contact with email '{email.Value}' already exists.");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.CategoryId);

            var subcategoryId = await ResolveSubcategoryAsync(category, request, cancellationToken);

            contact.Update(
                request.FirstName,
                request.LastName,
                email,
                phoneNumber,
                request.DateOfBirth,
                category.Id,
                subcategoryId);

            if (!string.IsNullOrEmpty(request.Password))
            {
                var password = new Password(request.Password);
                contact.ChangePasswordHash(_passwordHasher.Hash(password.Value));
            }

            await _contactRepository.UpdateAsync(contact, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Guid?> ResolveSubcategoryAsync(
            Category category,
            UpdateContactCommand request,
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
