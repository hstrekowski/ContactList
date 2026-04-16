namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when a request conflicts with the current state of the resource,
    /// for example attempting to create a contact with an email that is already taken.
    /// Mapped to HTTP 409 by the global exception handler in the API layer.
    /// </summary>
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
