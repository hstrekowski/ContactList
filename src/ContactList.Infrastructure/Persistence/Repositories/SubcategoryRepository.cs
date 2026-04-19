using ContactList.Application.Contracts.Persistence;
using ContactList.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of ISubcategoryRepository for managing subcategory data access.
/// </summary>
public sealed class SubcategoryRepository : ISubcategoryRepository
{
    private readonly ApplicationDbContext _db;

    public SubcategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Subcategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Subcategories.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IReadOnlyList<Subcategory>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _db.Subcategories
            .AsNoTracking()
            .Where(s => s.CategoryId == categoryId)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsInCategoryAsync(Guid subcategoryId, Guid categoryId, CancellationToken ct = default)
    {
        return await _db.Subcategories
            .AsNoTracking()
            .AnyAsync(s => s.Id == subcategoryId && s.CategoryId == categoryId, ct);
    }

    public async Task AddAsync(Subcategory subcategory, CancellationToken ct = default)
    {
        await _db.Subcategories.AddAsync(subcategory, ct);
    }

    public Task DeleteAsync(Subcategory subcategory, CancellationToken ct = default)
    {
        _db.Subcategories.Remove(subcategory);
        return Task.CompletedTask;
    }
}
