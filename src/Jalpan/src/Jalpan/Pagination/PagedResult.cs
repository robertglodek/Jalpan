namespace Jalpan.Pagination;

public class PagedResult<T> : PagedResultBase
{
    public IEnumerable<T> Items { get; }
    public bool IsEmpty => !Items.Any();
    public bool IsNotEmpty => !IsEmpty;

    protected PagedResult()
    {
        Items = [];
    }

    protected PagedResult(
        IEnumerable<T> items,
        int currentPage,
        int resultsPerPage,
        int totalPages,
        long totalResults):
        base(currentPage, resultsPerPage, totalPages, totalResults)
    {
        Items = items;
    }

    public static PagedResult<T> Create(
        IEnumerable<T> items,
        int currentPage,
        int resultsPerPage,
        int totalPages,
        long totalResults) 
        => new(items, currentPage, resultsPerPage, totalPages, totalResults);

    public static PagedResult<T> From(PagedResultBase result, IEnumerable<T> items)
        => new(items, result.CurrentPage, result.ResultsPerPage,
            result.TotalPages, result.TotalResults);

    public static PagedResult<T> Empty => new();

    public PagedResult<T1> Map<T1>(Func<T, T1> map)
        => PagedResult<T1>.From(this, Items.Select(map));
}
