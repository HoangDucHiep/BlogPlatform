using Lumo.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Lumo.Infrastructure.Repositories;

public abstract class Repository<T>
    where T : Entity
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

        return await _dbContext.Set<T>().FindAsync([id], cancellationToken);
    }

    public virtual void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

}
