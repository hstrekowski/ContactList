namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Throws on failed login and turns into a 401. Keeps the error generic so we don't leak if it was the email or password that failed.
    /// </summary>
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
