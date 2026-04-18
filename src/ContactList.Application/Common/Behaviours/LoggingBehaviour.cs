using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactList.Application.Common.Behaviours
{
    /// <summary>
    /// Logs when requests start and finish. Doesn't log payloads to keep passwords and sensitive data out of the logs.
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
