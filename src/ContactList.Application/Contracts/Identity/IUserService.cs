namespace ContactList.Application.Contracts.Identity
{
    /// <summary>
    /// Interface for identity operations. Handles user creation and signing in via JWT.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user and returns a token so they're logged in right away.
        /// </summary>
        /// <param name="request">New user credentials.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Auth data with JWT and user info.</returns>
        Task<AuthResponseDto> RegisterAsync(AuthRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks credentials and returns a JWT if they're correct. Returns null if login fails.
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Auth data on success, null on failure.</returns>
        Task<AuthResponseDto?> LoginAsync(AuthRequestDto request, CancellationToken cancellationToken = default);
    }
}
