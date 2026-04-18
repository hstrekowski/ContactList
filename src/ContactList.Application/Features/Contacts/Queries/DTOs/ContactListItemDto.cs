namespace ContactList.Application.Features.Contacts.Queries.DTOs
{
    /// <summary>
    /// A slim version of the contact for the main list view. Carries only the basic columns needed for the grid; full details like email or phone are fetched on demand.
    /// </summary>
    /// <param name="Id">Unique identifier used for navigating to the details page.</param>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="CategoryName">The display name of the contact's category.</param>
    public sealed record ContactListItemDto(
        Guid Id,
        string FirstName,
        string LastName,
        string CategoryName);
}
