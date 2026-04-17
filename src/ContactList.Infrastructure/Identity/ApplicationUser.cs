using Microsoft.AspNetCore.Identity;

namespace ContactList.Infrastructure.Identity;

/// <summary>
/// Represents an application user persisted by ASP.NET Core Identity.
/// Uses <see cref="Guid"/> as the primary key for consistency with domain aggregates.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
}
