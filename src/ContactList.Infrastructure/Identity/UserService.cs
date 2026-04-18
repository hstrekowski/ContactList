using ContactList.Application.Common.Exceptions;
using ContactList.Application.Contracts.Identity;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace ContactList.Infrastructure.Identity;

/// <summary>
/// ASP.NET Identity-backed implementation of <see cref="IUserService"/>.
/// Delegates credential storage and password verification to <see cref="UserManager{TUser}"/>
/// and issues JWT tokens via <see cref="JwtTokenService"/> on success.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenService _tokenService;

    public UserService(UserManager<ApplicationUser> userManager, JwtTokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(AuthRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            ThrowFromIdentityErrors(result.Errors);

        var (token, expiresAt) = _tokenService.GenerateToken(user.Id, user.Email!);
        return new AuthResponseDto(user.Id, user.Email!, token, expiresAt);
    }

    public async Task<AuthResponseDto?> LoginAsync(AuthRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return null;

        var passwordOk = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordOk)
            return null;

        var (token, expiresAt) = _tokenService.GenerateToken(user.Id, user.Email!);
        return new AuthResponseDto(user.Id, user.Email!, token, expiresAt);
    }

    private static void ThrowFromIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var errorList = errors.ToList();

        if (errorList.Any(e => IsDuplicate(e.Code)))
            throw new ConflictException("An account with this email already exists.");

        var passwordFailures = errorList
            .Where(e => IsPasswordPolicy(e.Code))
            .Select(e => new ValidationFailure("Password", e.Description))
            .ToList();

        if (passwordFailures.Count > 0)
            throw new ValidationException(passwordFailures);

        throw new InvalidOperationException(
            "Identity rejected user creation: " + string.Join("; ", errorList.Select(e => e.Description)));
    }

    private static bool IsDuplicate(string code)
        => code is nameof(IdentityErrorDescriber.DuplicateEmail)
              or nameof(IdentityErrorDescriber.DuplicateUserName);

    private static bool IsPasswordPolicy(string code)
        => code.StartsWith("Password", StringComparison.Ordinal);
}
