using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Login
{
    /// <summary>
    /// Authenticates an existing application user against the ASP.NET Identity store
    /// and returns a JWT on success. The handler treats wrong email and wrong password
    /// the same way (single generic error) so the response cannot be used to enumerate
    /// registered accounts.
    /// </summary>
    /// <param name="Email">Login email.</param>
    /// <param name="Password">Plain-text password.</param>
    public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
