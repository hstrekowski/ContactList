using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.DeleteContact
{
    /// <summary>
    /// Deletes a contact from the system. Throws a NotFoundException if the ID is not found, which results in a 404 response.
    /// </summary>
    /// <param name="Id">Unique identifier of the contact to remove.</param>
    public sealed record DeleteContactCommand(Guid Id) : IRequest;
}
