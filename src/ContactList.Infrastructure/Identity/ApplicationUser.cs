using Microsoft.AspNetCore.Identity;

namespace ContactList.Infrastructure.Identity;

/// <summary>
/// Represents a user in the Identity system. Uses Guid as the primary key to stay consistent with domain entities.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
}
