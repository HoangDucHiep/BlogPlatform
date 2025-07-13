namespace Lumo.Domain.Abstractions;


/// <summary>
/// Defines a contract for a unit of work that encapsulates a set of operations to be performed as a single transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
