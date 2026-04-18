using ContactList.Application.Contracts.Identity;
using ContactList.Application.Features.Auth.Commands.Login;
using ContactList.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Api.Controllers;

/// <summary>
/// Entry-point controller for unauthenticated users. Provides registration and
/// login; both endpoints return a JWT in <see cref="AuthResponseDto"/> that the
/// client must attach as a <c>Bearer</c> token on subsequent calls to protected
/// endpoints.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new application user and issues a JWT so the client can access
    /// protected endpoints without a separate login call.
    /// </summary>
    /// <response code="200">User created; response body contains the JWT.</response>
    /// <response code="400">Validation failed (weak password, invalid email, etc.).</response>
    /// <response code="409">An account with this email already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Authenticates an existing user and issues a JWT. Wrong email and wrong
    /// password produce the same 401 response to prevent account enumeration.
    /// </summary>
    /// <response code="200">Authentication succeeded; response body contains the JWT.</response>
    /// <response code="400">Validation failed (missing email or password).</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}
