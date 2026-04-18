using ContactList.Application.Contracts.Security;

namespace ContactList.Infrastructure.Security;

/// <summary>
/// BCrypt implementation of the password hasher. This is used for contact-specific passwords 
/// stored in the database, which are treated as data fields rather than system login credentials.
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
