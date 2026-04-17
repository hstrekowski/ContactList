using FluentValidation;
using MediatR;
using ValidationException = ContactList.Application.Common.Exceptions.ValidationException;

namespace ContactList.Application.Common.Behaviours
{
    /// <summary>
    /// MediatR pipeline step that runs every FluentValidation <see cref="IValidator{T}"/>
    /// registered for the incoming request. Collects the failures from all validators
    /// in parallel and throws a single <see cref="ValidationException"/> if any are found,
    /// preventing the handler from running with invalid input.
    /// </summary>
    public sealed class ValidationBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
