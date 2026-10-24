namespace IpDeputyApi.Paging;

public class PageRequest
{
    public const int DefaultPageSize = 20;
    public static readonly int[] AllowedPageSizes = [10, 20, 50, 100];

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;
    public string? Sort { get; set; }
    public string? Order { get; set; }
    public string? Q { get; set; }

    public bool IsDescending => string.Equals(Order, "descend", StringComparison.OrdinalIgnoreCase);

    public int GetPageSize()
    {
        if (PageSize <= 0)
            return DefaultPageSize;

        foreach (var size in AllowedPageSizes)
        {
            if (size >= PageSize)
                return size;
        }

        return AllowedPageSizes[^1];
    }
}
