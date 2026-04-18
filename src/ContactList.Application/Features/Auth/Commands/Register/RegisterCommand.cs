using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Register
{
    /// <summary>
    /// Registers a new user and gives them a JWT right away so they don't have to log in manually after signing up.
    /// </summary>
    /// <param name="Email">User's email, needs to be unique.</param>
    /// <param name="Password">Raw password that meets the complexity rules.</param>
    public sealed record RegisterCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
