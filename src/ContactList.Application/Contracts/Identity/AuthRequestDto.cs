namespace ContactList.Application.Contracts.Identity
{
    /// <summary>
    /// Credentials used for login and registration.
    /// </summary>
    /// <param name="Email">User's email.</param>
    /// <param name="Password">Raw password, gets hashed before saving.</param>
    public sealed record AuthRequestDto(string Email, string Password);
}
