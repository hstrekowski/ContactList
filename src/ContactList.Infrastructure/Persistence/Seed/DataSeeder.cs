using ContactList.Domain.Common;
using ContactList.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ContactList.Domain.ValueObjects;

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
        await SeedContactsAsync(db, ct);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedContactsAsync(ApplicationDbContext db, CancellationToken ct)
    {
        await EnsureContactAsync(db, DictionaryIds.Contacts.Contact1, "Seed_Jan", "Seed_Kowalski", "seed.jan@example.com", "hash", "+48111222333", new DateOnly(1985, 5, 15), DictionaryIds.Categories.Sluzbowy, DictionaryIds.SluzbowySubcategories.Szef, ct);
        await EnsureContactAsync(db, DictionaryIds.Contacts.Contact2, "Seed_Anna", "Seed_Nowak", "seed.anna@example.com", "hash", "+48999888777", new DateOnly(1992, 8, 20), DictionaryIds.Categories.Prywatny, null, ct);
        await EnsureContactAsync(db, DictionaryIds.Contacts.Contact3,"Seed_Marek", "Seed_Sąsiad", "seed.marek@example.com", "hash", "+48666777888", new DateOnly(1975, 3, 10), DictionaryIds.Categories.Prywatny, null, ct);
    }

    private static async Task EnsureContactAsync(ApplicationDbContext db, Guid id, string firstName, string lastName, string email, string passwordHash, string phoneNumber, DateOnly dateOfBirth, Guid categoryId, Guid? subcategoryId, CancellationToken ct)
    {
        var exists = await db.Contacts.AnyAsync(c => c.Id == id, ct);
        if (exists) return;

        var contact = new Contact(firstName, lastName, new Email(email), passwordHash, new PhoneNumber(phoneNumber), dateOfBirth, categoryId, subcategoryId);
        OverrideId(contact, id);
        await db.Contacts.AddAsync(contact, ct);
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
