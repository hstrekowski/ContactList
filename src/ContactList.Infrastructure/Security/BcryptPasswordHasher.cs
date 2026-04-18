using ContactList.Application.Contracts.Security;

namespace ContactList.Infrastructure.Security;

/// <summary>
/// BCrypt-backed implementation of <see cref="IPasswordHasher"/>.
/// Used to hash the <c>PasswordHash</c> field on <c>Contact</c> entities
/// (contact passwords are a data field per spec, not login credentials).
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 11;

    public string Hash(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password must not be empty.", nameof(plainPassword));

        return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
    }
}
