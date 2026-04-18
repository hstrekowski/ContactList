using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Login
{
    /// <summary>
    /// Checks credentials through the user service and returns a JWT. Throws a generic unauthorized exception if login fails to hide whether the email or password was wrong.
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
