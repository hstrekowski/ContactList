namespace ContactList.Application.Contracts.Identity
{
    /// <summary>
    /// Payload returned after successful login or registration. Holds the JWT token and basic user info.
    /// </summary>
    /// <param name="UserId">User's ID.</param>
    /// <param name="Email">User's email.</param>
    /// <param name="Token">The actual JWT token.</param>
    /// <param name="ExpiresAt">When the token expires (UTC).</param>
    public sealed record AuthResponseDto(
        Guid UserId,
        string Email,
        string Token,
        DateTime ExpiresAt);
}
