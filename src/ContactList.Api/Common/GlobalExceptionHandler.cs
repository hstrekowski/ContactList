using ContactList.Application.Common.Exceptions;
using ContactList.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Api.Common;

/// <summary>
/// Centralized exception handler that translates application and domain
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
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
