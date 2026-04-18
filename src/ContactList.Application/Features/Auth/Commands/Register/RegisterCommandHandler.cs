using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Register
{
    /// <summary>
    /// Handles user registration by delegating to the identity service.
    /// </summary>
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IUserService _userService;

        public RegisterCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken) =>
            _userService.RegisterAsync(new AuthRequestDto(request.Email, request.Password), cancellationToken);
    }
}
