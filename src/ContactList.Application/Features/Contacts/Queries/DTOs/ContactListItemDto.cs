namespace ContactList.Application.Features.Contacts.Queries.DTOs
{
    /// <summary>
    /// Slim projection of a contact returned on the paginated list screen.
    /// Carries only the columns shown in the grid; details are fetched separately.
    /// </summary>
    /// <param name="Id">Contact identifier.</param>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="Email">Email address (normalized lowercase).</param>
    /// <param name="PhoneNumber">Phone number in E.164 format.</param>
    /// <param name="CategoryName">Name of the assigned category (Służbowy / Prywatny / Inny).</param>
    public sealed record ContactListItemDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string CategoryName);
}
