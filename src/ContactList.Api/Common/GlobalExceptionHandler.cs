using ContactList.Application.Common.Exceptions;
using ContactList.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Api.Common;

/// <summary>
/// Centralized <see cref="IExceptionHandler"/> that translates application and domain
/// exceptions into RFC 7807 <see cref="ProblemDetails"/> responses.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Catches the exception, logs it with a severity matching the mapped HTTP status
    /// (Warning for 4xx, Error for 5xx), and writes a <see cref="ProblemDetails"/> body.
    /// Returns <c>false</c> if the response has already started so the framework can
    /// fall back to terminating the connection.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            _logger.LogWarning(
                "Response has already started; GlobalExceptionHandler cannot write a body for {ExceptionType}.",
                exception.GetType().Name);
            return false;
        }

        var (status, problem) = Map(exception);

        var logLevel = status >= StatusCodes.Status500InternalServerError
            ? LogLevel.Error
            : LogLevel.Warning;

        _logger.Log(
            logLevel,
            exception,
            "Handled {ExceptionType} → HTTP {StatusCode}",
            exception.GetType().Name,
            status);

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    /// <summary>
    /// Maps a caught exception to its HTTP status code and a populated
    /// <see cref="ProblemDetails"/> instance. Unknown exceptions collapse to a
    /// generic 500 without leaking the underlying message.
    /// </summary>
    private static (int Status, ProblemDetails Problem) Map(Exception exception) => exception switch
    {
        NotFoundException nf => (
            StatusCodes.Status404NotFound,
            new ProblemDetails
            {
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound,
                Detail = nf.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            }),

        ConflictException cf => (
            StatusCodes.Status409Conflict,
            new ProblemDetails
            {
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = cf.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            }),

        UnauthorizedException ua => (
            StatusCodes.Status401Unauthorized,
            new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = ua.Message,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            }),

        ValidationException ve => (
            StatusCodes.Status400BadRequest,
            new ValidationProblemDetails(ve.Errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            }),

        DomainException de => (
            StatusCodes.Status400BadRequest,
            new ProblemDetails
            {
                Title = "Invalid domain state",
                Status = StatusCodes.Status400BadRequest,
                Detail = de.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            }),

        _ => (
            StatusCodes.Status500InternalServerError,
            new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            }),
    };
}
