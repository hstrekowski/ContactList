using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Persistence;
using ContactList.Domain.Entities;
using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.DeleteContact
{
    /// <summary>
    /// Deletes a contact. We fetch it first to ensure it actually exists, allowing us to throw a 404 if it's missing rather than letting EF fail silently.
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
