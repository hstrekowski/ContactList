using ContactList.Application.Contracts.Persistence;
using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IContactRepository. Eagerly loads Category and Subcategory navigation properties to allow the Application layer to map them directly into DTOs.
/// </summary>
public sealed class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _db;

    public ContactRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Contacts
            .Include(c => c.Category)
            .Include(c => c.Subcategory)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Contact>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Contacts
            .Include(c => c.Category)
            .Include(c => c.Subcategory)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, Guid? excludeContactId = null, CancellationToken ct = default)
    {
        var query = _db.Contacts.AsNoTracking().Where(c => c.Email == email);

        if (excludeContactId.HasValue)
            query = query.Where(c => c.Id != excludeContactId.Value);

        return await query.AnyAsync(ct);
    }

    public async Task AddAsync(Contact contact, CancellationToken ct = default)
    {
        await _db.Contacts.AddAsync(contact, ct);
    }

    public Task UpdateAsync(Contact contact, CancellationToken ct = default)
    {
        _db.Contacts.Update(contact);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Contact contact, CancellationToken ct = default)
    {
        _db.Contacts.Remove(contact);
        return Task.CompletedTask;
    }
}
