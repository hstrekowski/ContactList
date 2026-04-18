using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Login
{
    /// <summary>
    /// Logs in the user and gives back a JWT. Errors are kept vague so people can't guess if an email exists in our db.
    /// </summary>
    /// <param name="Email">User's email.</param>
    /// <param name="Password">Raw password to check.</param>
    public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
