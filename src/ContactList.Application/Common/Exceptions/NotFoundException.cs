namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when a requested entity cannot be found in the data store.
    /// Mapped to HTTP 404 by the global exception handler in the API layer.
    /// </summary>
    public sealed class NotFoundException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="NotFoundException"/> with a free-form message.
        /// </summary>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        /// Convenience constructor producing a message in the form
        /// "<paramref name="entityName"/> with id '<paramref name="key"/>' was not found".
        /// </summary>
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with id '{key}' was not found.") { }
    }
}
