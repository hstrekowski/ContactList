namespace ContactList.Application.Features.Contacts.Queries.DTOs
{
    /// <summary>
    /// Full projection of a single contact shown on the details screen.
    /// The contact's password hash is intentionally NOT exposed here.
    /// </summary>
    /// <param name="Id">Contact identifier.</param>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="Email">Email address (normalized lowercase).</param>
    /// <param name="PhoneNumber">Phone number in E.164 format.</param>
    /// <param name="DateOfBirth">Contact's date of birth.</param>
    /// <param name="CategoryId">Identifier of the assigned category.</param>
    /// <param name="CategoryName">Name of the assigned category.</param>
    /// <param name="SubcategoryId">Identifier of the assigned subcategory, if any.</param>
    /// <param name="SubcategoryName">Name of the assigned subcategory, if any.</param>
    public sealed record ContactDetailDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        DateOnly DateOfBirth,
        Guid CategoryId,
        string CategoryName,
        Guid? SubcategoryId,
        string? SubcategoryName);
}
