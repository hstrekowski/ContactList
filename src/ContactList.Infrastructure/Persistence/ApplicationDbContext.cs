using ContactList.Domain.Entities;
using ContactList.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Persistence;

/// <summary>
/// Primary EF Core context for the application.
/// Combines ASP.NET Identity user stores (no role tables — single-user scenario)
/// with the contact list aggregates (<see cref="Contact"/>, <see cref="Category"/>, <see cref="Subcategory"/>).
/// </summary>
public class ApplicationDbContext : IdentityUserContext<ApplicationUser, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subcategory> Subcategories => Set<Subcategory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
