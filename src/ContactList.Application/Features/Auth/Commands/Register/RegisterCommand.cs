using ContactList.Application.Contracts.Identity;
using MediatR;

namespace ContactList.Application.Features.Auth.Commands.Register
{
    /// <summary>
    /// Registers a new application user (ASP.NET Identity <c>ApplicationUser</c>) and
    /// immediately issues a JWT so the client can use the protected endpoints without
    /// a separate login round trip.
    /// </summary>
    /// <param name="Email">Login email — must be unique among application users.</param>
    /// <param name="Password">Plain-text password meeting complexity rules.</param>
    public sealed record RegisterCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
