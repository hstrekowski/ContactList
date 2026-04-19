using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Domain.Entities;
using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.DeleteContact
{
    /// <summary>
    /// Deletes a contact. If the contact belongs to the 'Inny' category, its custom subcategory is deleted as well — those subcategories are owned by a single contact and would otherwise become orphans.
    /// </summary>
    public sealed class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand>
    {
        private const string OtherCategoryName = "Inny";

        private readonly IContactRepository _contactRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteContactCommandHandler(
            IContactRepository contactRepository,
            ISubcategoryRepository subcategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _subcategoryRepository = subcategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Contact), request.Id);

            var customSubcategory = contact.Category.Name == OtherCategoryName
                ? contact.Subcategory
                : null;

            await _contactRepository.DeleteAsync(contact, cancellationToken);

            if (customSubcategory is not null)
            {
                await _subcategoryRepository.DeleteAsync(customSubcategory, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
