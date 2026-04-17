namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when the caller could not be authenticated — for example wrong email
    /// or password during login. Mapped to HTTP 401 by the global exception handler
    /// in the API layer. Carries a single generic message to avoid leaking which
    /// half of the credential pair was wrong (anti-enumeration).
    /// </summary>
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
