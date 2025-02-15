using Jalpan.Pagination;
using Taskly.Services.Meta.Application.DTO;

namespace Taskly.Services.Meta.Application.Queries;

public sealed class SearchSections : PagedQueryBase, IQuery<PagedResult<SectionDto>>
{
    public Guid? GoalId { get; set; }
}