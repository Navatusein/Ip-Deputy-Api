using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Paging;

public static class PagingExtensions
{
    public static async Task<PageResult<TDto>> ToPageResultAsync<T, TDto>(
        this IQueryable<T> query,
        PageRequest request,
        Func<IQueryable<T>, IQueryable<TDto>> projection,
        CancellationToken cancellationToken)
    {
        var total = await query.CountAsync(cancellationToken);
        var window = PageWindow.Calculate(total, request.Page, request.GetPageSize());

        var items = await projection(query.Skip(window.Skip).Take(window.PageSize)).ToListAsync(cancellationToken);

        return new PageResult<TDto>
        {
            Items = items,
            Total = total,
            Page = window.Page,
            PageSize = window.PageSize
        };
    }
}
