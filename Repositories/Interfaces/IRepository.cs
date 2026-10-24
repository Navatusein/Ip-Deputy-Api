using IpDeputyApi.Database.Models;

namespace IpDeputyApi.Repositories.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    IQueryable<T> Query();
    IQueryable<T> QueryTracked();
    Task<T?> FindAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<int> CountExistingAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<int> DeleteByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
}
