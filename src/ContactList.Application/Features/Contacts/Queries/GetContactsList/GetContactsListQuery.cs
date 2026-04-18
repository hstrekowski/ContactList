using ContactList.Application.Features.Contacts.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactsList
{
    /// <summary>
    /// Returns all contacts using the slim list DTO for the main grid view.
    /// </summary>
    public sealed record GetContactsListQuery : IRequest<IReadOnlyList<ContactListItemDto>>;
}
