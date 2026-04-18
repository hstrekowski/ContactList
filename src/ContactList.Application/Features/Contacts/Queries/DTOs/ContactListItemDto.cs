namespace ContactList.Application.Features.Contacts.Queries.DTOs
{
    /// <summary>
    /// Slim projection of a contact returned on the list screen.
    /// Carries only the columns shown in the grid; full details (email, phone,
    /// date of birth, subcategory) are fetched separately via GET /api/contacts/{id}.
    /// </summary>
    /// <param name="Id">Contact identifier — used by the UI to navigate to the details view.</param>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="CategoryName">Name of the assigned category (Służbowy / Prywatny / Inny).</param>
    public sealed record ContactListItemDto(
        Guid Id,
        string FirstName,
        string LastName,
        string CategoryName);
}
