namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Throws when an entity is missing from the database. Caught by middleware to return a 404.
    /// </summary>
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with id '{key}' was not found.") { }
    }
}
