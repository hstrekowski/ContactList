namespace ContactList.Application.Contracts.Identity
{
    /// <summary>
    /// Result returned after a successful authentication (register or login).
    /// Contains the issued JWT and basic user metadata needed by the client.
    /// </summary>
    /// <param name="UserId">Identifier of the authenticated user.</param>
    /// <param name="Email">Email address of the authenticated user.</param>
    /// <param name="Token">Issued JWT bearer token.</param>
    /// <param name="ExpiresAt">UTC timestamp at which the token expires.</param>
    public sealed record AuthResponseDto(
        Guid UserId,
        string Email,
        string Token,
        DateTime ExpiresAt);
}
