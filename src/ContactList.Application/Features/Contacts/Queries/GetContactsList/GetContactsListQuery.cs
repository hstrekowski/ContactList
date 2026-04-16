using ContactList.Application.Features.Contacts.Queries.DTOs;
using MediatR;

namespace ContactList.Application.Features.Contacts.Queries.GetContactsList
{
    /// <summary>
    /// Returns every contact projected to the slim <see cref="ContactListItemDto"/> shape
    /// shown on the list screen. No filtering or pagination — the recruitment spec only
    /// asks for "przeglądanie listy kontaktów" and the dataset is expected to stay small.
    /// </summary>
    public sealed record GetContactsListQuery : IRequest<IReadOnlyList<ContactListItemDto>>;
}
