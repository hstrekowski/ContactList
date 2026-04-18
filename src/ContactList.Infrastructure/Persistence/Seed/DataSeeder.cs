using ContactList.Domain.Common;
using ContactList.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Persistence.Seed;

/// <summary>
/// Idempotent database seeder for dictionary data.
/// Safe to run on every application startup — existing rows are detected by id.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        await SeedCategoriesAsync(db, ct);
        await SeedSluzbowySubcategoriesAsync(db, ct);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        await EnsureCategoryAsync(db, DictionaryIds.Categories.Sluzbowy, "Służbowy", ct);
        await EnsureCategoryAsync(db, DictionaryIds.Categories.Prywatny, "Prywatny", ct);
        await EnsureCategoryAsync(db, DictionaryIds.Categories.Inny,     "Inny",     ct);
    }

    private static async Task SeedSluzbowySubcategoriesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        var categoryId = DictionaryIds.Categories.Sluzbowy;
        await EnsureSubcategoryAsync(db, DictionaryIds.SluzbowySubcategories.Szef,           "Szef",            categoryId, ct);
        await EnsureSubcategoryAsync(db, DictionaryIds.SluzbowySubcategories.Klient,         "Klient",          categoryId, ct);
        await EnsureSubcategoryAsync(db, DictionaryIds.SluzbowySubcategories.Wspolpracownik, "Współpracownik",  categoryId, ct);
        await EnsureSubcategoryAsync(db, DictionaryIds.SluzbowySubcategories.Partner,        "Partner",         categoryId, ct);
    }

    private static async Task EnsureCategoryAsync(ApplicationDbContext db, Guid id, string name, CancellationToken ct)
    {
        var exists = await db.Categories.AnyAsync(c => c.Id == id, ct);
        if (exists)
            return;

        var category = new Category(name);
        OverrideId(category, id);
        await db.Categories.AddAsync(category, ct);
    }

    private static async Task EnsureSubcategoryAsync(
        ApplicationDbContext db,
        Guid id,
        string name,
        Guid categoryId,
        CancellationToken ct)
    {
        var exists = await db.Subcategories.AnyAsync(s => s.Id == id, ct);
        if (exists)
            return;

        var subcategory = new Subcategory(name, categoryId);
        OverrideId(subcategory, id);
        await db.Subcategories.AddAsync(subcategory, ct);
    }

    /// <summary>
    /// Replaces the auto-generated <see cref="BaseEntity.Id"/> with a deterministic value.
    /// Uses reflection because the domain exposes the setter as <c>protected</c>; seeding is
    /// the single legitimate caller that needs stable dictionary identifiers.
    /// </summary>
    private static void OverrideId(BaseEntity entity, Guid id)
    {
        typeof(BaseEntity)
            .GetProperty(nameof(BaseEntity.Id))!
            .SetValue(entity, id);
    }
}
