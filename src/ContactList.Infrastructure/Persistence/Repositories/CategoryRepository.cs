using ContactList.Application.Contracts.Persistence;
using ContactList.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ICategoryRepository"/>.
/// Reads eagerly include <see cref="Subcategory"/> entries so callers can
/// project both dictionary levels in a single round-trip.
/// </summary>
public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .Include(c => c.Subcategories)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _db.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
