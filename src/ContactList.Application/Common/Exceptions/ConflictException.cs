namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Throws when something conflicts with existing data, like a duplicate email. Gets caught and turned into a 409 Conflict.
    /// </summary>
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
