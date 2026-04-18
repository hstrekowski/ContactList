using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.UpdateContact
{
    /// <summary>
    /// Updates an existing contact. If the password is provided, it's hashed and updated; otherwise, the existing one stays. All other fields overwrite the current record.
    /// </summary>
    /// <param name="Id">ID of the contact to update.</param>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="Email">New email, must remain unique across the system.</param>
    /// <param name="Password">Optional new password. Leave null to keep the current one.</param>
    /// <param name="PhoneNumber">Phone number in E.164 format.</param>
    /// <param name="DateOfBirth">Date of birth.</param>
    /// <param name="CategoryId">ID of the chosen category.</param>
    /// <param name="SubcategoryId">Subcategory ID for 'Służbowy' contacts.</param>
    /// <param name="SubcategoryName">Custom name for 'Inny' contacts.</param>
    public sealed record UpdateContactCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? Password,
        string PhoneNumber,
        DateOnly DateOfBirth,
        Guid CategoryId,
        Guid? SubcategoryId,
        string? SubcategoryName) : IRequest;
}
