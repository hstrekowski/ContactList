namespace ContactList.Application.Contracts.Identity
{
    /// <summary>
    /// Abstraction over the ASP.NET Identity user store used by the authentication features.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user with the given credentials and issues a JWT for the new user
        /// so the client is authenticated immediately after registration.
        /// </summary>
        /// <param name="request">Email and password for the new account.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>Authentication result containing the issued JWT and user metadata.</returns>
        /// <exception cref="Common.Exceptions.ConflictException">Email is already taken.</exception>
        /// <exception cref="Common.Exceptions.ValidationException">Identity rejected the password (policy violation).</exception>
        Task<AuthResponseDto> RegisterAsync(AuthRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates the credentials and, on success, issues a JWT for the user.
        /// Returns <c>null</c> when the email is unknown or the password is wrong;
        /// the handler translates that into an unauthorized response.
        /// </summary>
        /// <param name="request">Email and password submitted by the client.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>Authentication result on success; <c>null</c> when credentials are invalid.</returns>
        Task<AuthResponseDto?> LoginAsync(AuthRequestDto request, CancellationToken cancellationToken = default);
    }
}
