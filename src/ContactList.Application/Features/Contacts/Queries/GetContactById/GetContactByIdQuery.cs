using ContactList.Application.Features.Contacts.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactById
{
    /// <summary>
    /// Fetches the full details of a specific contact. Throws a NotFoundException if the ID is not found.
    /// </summary>
    /// <param name="Id">The unique ID of the contact to fetch.</param>
    public sealed record GetContactByIdQuery(Guid Id) : IRequest<ContactDetailDto>;
}
