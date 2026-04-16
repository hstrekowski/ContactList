using ContactList.Domain.Entities;
using ContactList.Domain.ValueObjects;

namespace ContactList.Application.Contracts.Persistence
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Contact>> GetAllAsync(CancellationToken CT = default);
        Task<bool> ExistsByEmailAsync(Email email, Guid? excludeContactId = null, CancellationToken ct = default);
        Task AddAsync(Contact contact, CancellationToken ct = default);
        Task UpdateAsync(Contact contact, CancellationToken ct = default);
        Task DeleteAsync(Contact contact, CancellationToken ct = default);
    }
}
