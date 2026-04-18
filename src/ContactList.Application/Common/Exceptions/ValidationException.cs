using FluentValidation.Results;

namespace ContactList.Application.Common.Exceptions
{
    /// <summary>
    /// Groups up all the FluentValidation errors caught by the pipeline. Turns into a 400 Bad Request in the middleware.
    /// </summary>
    public sealed class ValidationException : Exception
    {
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
