using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : class, IEntity
{
    protected readonly IpDeputyDbContext Context;
    protected readonly DbSet<T> Set;

    public RepositoryBase(IpDeputyDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public IQueryable<T> Query()
    {
        return Set.AsNoTracking();
    }

    public IQueryable<T> QueryTracked()
    {
        return Set;
    }

    public Task<T?> FindAsync(int id, CancellationToken cancellationToken)
    {
        return Set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return Set.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public Task<int> CountExistingAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        return Set.CountAsync(x => ids.Contains(x.Id), cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await Set.AddAsync(entity, cancellationToken);
    }

    public void Remove(T entity)
    {
        Set.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        Set.RemoveRange(entities);
    }

    public Task<int> DeleteByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        return Set.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
    }
}
