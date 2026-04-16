namespace ContactList.Application.Contracts.Identity
{
    /// <summary>
    /// Credentials payload accepted by the authentication service for both
    /// registration and login operations.
    /// </summary>
    /// <param name="Email">Email address used as the login identifier.</param>
    /// <param name="Password">Plain-text password (hashed by the identity store before persistence).</param>
    public sealed record AuthRequestDto(string Email, string Password);
}
