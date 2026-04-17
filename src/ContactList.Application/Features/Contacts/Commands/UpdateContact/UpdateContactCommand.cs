using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.UpdateContact
{
    /// <summary>
    /// Updates an existing contact. <see cref="Password"/> is optional — when null the
    /// stored hash is left untouched; when provided the new value is hashed and replaces
    /// the previous hash. All other fields are required and overwrite the current state.
    /// </summary>
    /// <param name="Id">Identifier of the contact to update.</param>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="Email">Email address — must be unique across all other contacts.</param>
    /// <param name="Password">Optional new plain-text password; null leaves the existing hash in place.</param>
    /// <param name="PhoneNumber">Phone number in E.164 format.</param>
    /// <param name="DateOfBirth">Contact's date of birth.</param>
    /// <param name="CategoryId">Identifier of the chosen category (Służbowy / Prywatny / Inny).</param>
    /// <param name="SubcategoryId">
    /// Identifier of an existing subcategory — required when the category is "Służbowy",
    /// must be null otherwise.
    /// </param>
    /// <param name="SubcategoryName">
    /// Free-form subcategory name — required when the category is "Inny", must be null otherwise.
    /// </param>
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
