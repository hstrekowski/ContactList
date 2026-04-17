using ContactList.Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactList.Application.Common.Behaviours
{
    /// <summary>
    /// Outer-most MediatR pipeline step. Catches any exception thrown by the rest of the pipeline
    /// (handler, validation, logging), logs unexpected ones and re-throws so the API layer can
    /// translate the exception to the proper HTTP response.
    /// </summary>
    public sealed class UnhandledExceptionBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception for request {RequestName}",
                    typeof(TRequest).Name);
                throw;
            }
        }
    }
}
