namespace ContactList.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a domain rule is violated.
    /// </summary>
    public sealed class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
