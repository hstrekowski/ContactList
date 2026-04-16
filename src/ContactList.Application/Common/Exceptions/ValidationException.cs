using FluentValidation.Results;

namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Aggregates one or more validation failures collected from FluentValidation validators
    /// executed by the <c>ValidationBehaviour</c> pipeline step.
    /// Mapped to HTTP 400 by the global exception handler in the API layer.
    /// </summary>
    public sealed class ValidationException : Exception
    {
        /// <summary>
        /// Failures grouped by property name. Each property maps to the list of messages for that property.
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}
