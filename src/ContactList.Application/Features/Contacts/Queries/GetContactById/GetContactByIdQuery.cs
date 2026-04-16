using ContactList.Application.Features.Contacts.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactById
{
    /// <summary>
    /// Returns the full <see cref="ContactDetailDto"/> projection for a single contact.
    /// The handler throws <c>NotFoundException</c> when the id does not match a row,
    /// so the response type is non-nullable.
    /// </summary>
    /// <param name="Id">Identifier of the contact to fetch.</param>
    public sealed record GetContactByIdQuery(Guid Id) : IRequest<ContactDetailDto>;
}
