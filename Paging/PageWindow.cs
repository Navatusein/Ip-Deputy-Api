namespace IpDeputyApi.Paging;

public readonly record struct PageWindow(int Page, int PageSize, int Skip)
{
    public static PageWindow Calculate(int total, int requestedPage, int pageSize)
    {
        var lastPage = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        var page = Math.Clamp(requestedPage, 1, lastPage);

        return new PageWindow(page, pageSize, (page - 1) * pageSize);
    }
}
