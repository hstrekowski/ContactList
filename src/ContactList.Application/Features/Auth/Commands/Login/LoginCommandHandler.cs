using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Login
{
    /// <summary>
    /// Verifies credentials through <see cref="IUserService.LoginAsync"/> and returns
    /// an issued JWT. A null result from the service means either the email or the
    /// password was wrong — both cases collapse into a single
    /// <see cref="UnauthorizedException"/> with a generic message, which the API
    /// layer maps to HTTP 401. Treating both failure modes identically prevents
    /// account-enumeration attacks that probe whether a given email is registered.
    /// </summary>
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserService _userService;

        public LoginCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = await _userService.LoginAsync(
                new AuthRequestDto(request.Email, request.Password),
                cancellationToken);

            return response ?? throw new UnauthorizedException("Invalid email or password.");
        }
    }
}
