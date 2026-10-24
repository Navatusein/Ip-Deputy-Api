using IpDeputyApi.Database;
using IpDeputyApi.Repositories.Interfaces;

namespace IpDeputyApi.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IpDeputyDbContext _context;

    public UnitOfWork(IpDeputyDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        if (_context.Database.CurrentTransaction != null)
            return await action(cancellationToken);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var result = await action(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return result;
    }

    public Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        return ExecuteInTransactionAsync(async token =>
        {
            await action(token);
            return true;
        }, cancellationToken);
    }
}
