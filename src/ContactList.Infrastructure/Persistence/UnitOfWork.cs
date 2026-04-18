using ContactList.Application.Contracts.Persistence;

namespace ContactList.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of the Unit of Work pattern. It commits all repository changes tracked by the database context in a single transaction.
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
