using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.DeleteContact
{
    /// <summary>
    /// Removes a contact from the store. The handler throws <c>NotFoundException</c>
    /// when the id does not match an existing row, so the API layer can translate
    /// that into a 404 response.
    /// </summary>
    /// <param name="Id">Identifier of the contact to delete.</param>
    public sealed record DeleteContactCommand(Guid Id) : IRequest;
}
