using Jalpan.Types;

namespace Jalpan.Pagination;

public interface IPagedQuery : IQuery
{
    int Page { get; }
    int Results { get; }
    string? OrderBy { get; }
    string? SortOrder { get; }
}
