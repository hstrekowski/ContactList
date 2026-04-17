using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Domain.Entities;
using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.DeleteContact
{
    /// <summary>
    /// Loads the contact, asks the repository to remove it, and commits the change.
    /// We deliberately fetch first so the API can distinguish "no such contact" (404)
    /// from "deleted nothing because the row was already gone" — without the read,
    /// EF would silently succeed.
    /// </summary>
    public sealed class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteContactCommandHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Contact), request.Id);

            await _contactRepository.DeleteAsync(contact, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
