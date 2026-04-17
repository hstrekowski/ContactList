namespace ContactList.Infrastructure.Identity;

/// <summary>
/// Strongly-typed JWT configuration bound from the <c>Jwt</c> section of application configuration.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>Token issuer (<c>iss</c> claim).</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>Expected audience (<c>aud</c> claim).</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Symmetric signing key; must be at least 32 characters for HMAC-SHA256.</summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>Token lifetime in minutes.</summary>
    public int ExpiresInMinutes { get; init; } = 60;
}
