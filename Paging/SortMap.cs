using System.Linq.Expressions;
using IpDeputyApi.Database.Models;

namespace IpDeputyApi.Paging;

public class SortMap<T> where T : IEntity
{
    private readonly Dictionary<string, Func<IQueryable<T>, bool, IOrderedQueryable<T>>> _sorts = new(StringComparer.OrdinalIgnoreCase);
    private Func<IQueryable<T>, IOrderedQueryable<T>> _default = query => query.OrderBy(x => x.Id);

    public IReadOnlyCollection<string> Keys => _sorts.Keys;

    public SortMap<T> Add<TKey>(string key, Expression<Func<T, TKey>> selector)
    {
        _sorts[key] = (query, descending) => descending ? query.OrderByDescending(selector) : query.OrderBy(selector);
        return this;
    }

    public SortMap<T> Default(Func<IQueryable<T>, IOrderedQueryable<T>> apply)
    {
        _default = apply;
        return this;
    }

    public IOrderedQueryable<T> Apply(IQueryable<T> query, string? key, bool descending)
    {
        if (key != null && _sorts.TryGetValue(key, out var sort))
            return sort(query, descending).ThenBy(x => x.Id);

        return _default(query).ThenBy(x => x.Id);
    }

    public IOrderedQueryable<T> ApplyDefault(IQueryable<T> query)
    {
        return _default(query).ThenBy(x => x.Id);
    }
}
