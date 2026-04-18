using ContactList.Application.Contracts.Persistence;

namespace ContactList.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// Delegates to <see cref="ApplicationDbContext.SaveChangesAsync(CancellationToken)"/>,
/// which commits all repository mutations tracked on the shared context
/// in a single transaction.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
