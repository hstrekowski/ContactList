namespace ContactList.Application.Contracts.Identity
{
    /// <summary>
    /// Issues signed JWT bearer tokens for authenticated users.
    /// Implementation lives in the infrastructure layer to keep JWT
    /// signing configuration out of the application layer.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a signed JWT for the given user.
        /// </summary>
        /// <param name="userId">Identifier of the user.</param>
        /// <param name="email">Email address placed in the token claims.</param>
        /// <returns>Token string together with its UTC expiration timestamp.</returns>
        (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string email);
    }
}
