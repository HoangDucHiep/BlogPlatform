using Lumo.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lumo.Infrastructure.Repositories;

public abstract class Repository<T> where T : Entity
{
    protected readonly ApplicationDbContext _dbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty.", nameof(id));
        }

        return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (OperationCanceledException)
        {
            throw; // Chuyển tiếp OperationCanceledException để tầng trên xử lý
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryException("Failed to add entity to database.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("An unexpected error occurred while adding entity.", ex);
        }
    }
}

public class RepositoryException : Exception
{
    public RepositoryException(string message, Exception innerException)
        : base(message, innerException) { }
}
