using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Register
{
    /// <summary>
    /// Thin orchestration wrapper around <see cref="IUserService.RegisterAsync"/>.
    /// The actual ASP.NET Identity work — creating the <c>ApplicationUser</c>, hashing
    /// the password, mapping Identity errors to <c>ConflictException</c> /
    /// <c>ValidationException</c>, and minting the JWT — lives in the Infrastructure
    /// implementation so the Application layer stays free of Identity dependencies.
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
