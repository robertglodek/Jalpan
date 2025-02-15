using Jalpan.Pagination;
using Taskly.Services.Meta.Application.DTO;

namespace Taskly.Services.Meta.Application.Queries;

public sealed class SearchTags : PagedQueryBase, IQuery<PagedResult<TagDto>>
{
    
}