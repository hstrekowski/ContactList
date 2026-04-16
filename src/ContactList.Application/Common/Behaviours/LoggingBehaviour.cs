using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactList.Application.Common.Behaviours
{
    /// <summary>
    /// MediatR pipeline step that logs the entry and successful exit of every request,
    /// giving observability into which commands and queries were executed.
    /// Request payloads are intentionally NOT logged to avoid leaking sensitive data
    /// such as passwords from <c>AuthRequestDto</c>.
    /// </summary>
    public sealed class LoggingBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Handling {RequestName}", requestName);
            var response = await next();
            _logger.LogInformation("Handled {RequestName}", requestName);

            return response;
        }
    }
}
