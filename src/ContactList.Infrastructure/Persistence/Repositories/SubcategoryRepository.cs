using ContactList.Application.Contracts.Persistence;
using ContactList.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactList.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ISubcategoryRepository"/>.
/// </summary>
public sealed class SubcategoryRepository : ISubcategoryRepository
{
    private readonly ApplicationDbContext _db;

    public SubcategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Subcategory>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _db.Subcategories
            .AsNoTracking()
            .Where(s => s.CategoryId == categoryId)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
    }

    public Task<bool> ExistsInCategoryAsync(Guid subcategoryId, Guid categoryId, CancellationToken ct = default)
    {
        return _db.Subcategories
            .AsNoTracking()
            .AnyAsync(s => s.Id == subcategoryId && s.CategoryId == categoryId, ct);
    }

    public async Task AddAsync(Subcategory subcategory, CancellationToken ct = default)
    {
        await _db.Subcategories.AddAsync(subcategory, ct);
    }
}
