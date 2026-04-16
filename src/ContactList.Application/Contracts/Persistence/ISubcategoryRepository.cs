using ContactList.Domain.Entities;

namespace ContactList.Application.Contracts.Persistence
{
    public interface ISubcategoryRepository
    {
        Task<Subcategory?> GetByIdAsync(Guid id, CancellationToken ct = default); 
        Task<IReadOnlyList<Subcategory>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default);
        Task<bool> ExistsInCategoryAsync(Guid subcategoryId, Guid categoryId, CancellationToken ct = default);
        Task AddAsync(Subcategory subcategory, CancellationToken ct = default);
    }
}
