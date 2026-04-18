namespace ContactList.Application.Features.Contacts.Queries.DTOs
{
    /// <summary>
    /// Detailed contact information for the UI. Password hashes are excluded for security.
    /// </summary>
    /// <param name="Id">Unique ID.</param>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="Email">Email address.</param>
    /// <param name="PhoneNumber">Phone number.</param>
    /// <param name="DateOfBirth">Date of birth.</param>
    /// <param name="CategoryId">ID of the category.</param>
    /// <param name="CategoryName">Name of the category.</param>
    /// <param name="SubcategoryId">ID of the subcategory, if set.</param>
    /// <param name="SubcategoryName">Name of the subcategory, if set.</param>
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
