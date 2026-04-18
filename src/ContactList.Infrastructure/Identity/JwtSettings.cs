namespace ContactList.Infrastructure.Identity;

/// <summary>
/// Strongly-typed JWT configuration settings, mapped from the application's configuration file.
/// </summary>
public sealed class JwtSettings
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public int ExpiresInMinutes { get; init; } = 60;
}
