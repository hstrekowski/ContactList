using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.CreateContact
{
    /// <summary>
    /// Creates a new contact. Returns the identifier of the inserted row so the API
    /// layer can produce a 201 Created response with a Location header.
    /// </summary>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="Email">Email address — must be unique across all contacts.</param>
    /// <param name="Password">Plain-text password meeting complexity rules; hashed before persistence.</param>
    /// <param name="PhoneNumber">Phone number in E.164 format.</param>
    /// <param name="DateOfBirth">Contact's date of birth.</param>
    /// <param name="CategoryId">Identifier of the chosen category (Służbowy / Prywatny / Inny).</param>
    /// <param name="SubcategoryId">
    /// Identifier of an existing subcategory — required when the category is "Służbowy",
    /// must be null otherwise.
    /// </param>
    /// <param name="SubcategoryName">
    /// Free-form subcategory name — required when the category is "Inny", must be null otherwise.
    /// The handler will create a new <c>Subcategory</c> row under "Inny" using this value.
    /// </param>
    public sealed record CreateContactCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string PhoneNumber,
        DateOnly DateOfBirth,
        Guid CategoryId,
        Guid? SubcategoryId,
        string? SubcategoryName) : IRequest<Guid>;
}
