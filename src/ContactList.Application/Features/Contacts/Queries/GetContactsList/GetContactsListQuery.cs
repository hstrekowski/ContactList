using ContactList.Application.Features.Contacts.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactsList
{
    /// <summary>
    /// Returns every contact projected to the slim <see cref="ContactListItemDto"/> shape
    /// shown on the list screen.
    /// </summary>
    public sealed record GetContactsListQuery : IRequest<IReadOnlyList<ContactListItemDto>>;
}
