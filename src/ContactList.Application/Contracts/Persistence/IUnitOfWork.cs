namespace ContactList.Application.Contracts.Persistence
{
    /// <summary>
    /// Coordinates committing tracked changes from repositories to the database
    /// as a single atomic transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Persists all pending changes tracked by the repositories within one transaction.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
