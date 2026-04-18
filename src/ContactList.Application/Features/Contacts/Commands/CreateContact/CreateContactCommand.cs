using MediatR;

namespace ContactList.Application.Features.Contacts.Commands.CreateContact
{
    /// <summary>
    /// Creates a new contact and returns its unique identifier.
    /// </summary>
    /// <param name="FirstName">First name.</param>
    /// <param name="LastName">Last name.</param>
    /// <param name="Email">Unique email address.</param>
    /// <param name="Password">Plain-text password that gets hashed before saving.</param>
    /// <param name="PhoneNumber">Phone number in E.164 format.</param>
    /// <param name="DateOfBirth">Date of birth.</param>
    /// <param name="CategoryId">ID of the selected category.</param>
    /// <param name="SubcategoryId">ID of an existing subcategory, used for the 'Służbowy' category.</param>
    /// <param name="SubcategoryName">Custom subcategory name, used for the 'Inny' category.</param>
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
