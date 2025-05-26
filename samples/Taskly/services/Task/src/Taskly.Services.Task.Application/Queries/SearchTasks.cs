using Jalpan.Pagination;
using Jalpan.Types;
using Taskly.Services.Task.Application.DTO;

namespace Taskly.Services.Task.Application.Queries;

public sealed class SearchTasks : PagedQueryBase, IQuery<PagedResult<TaskDto>>
{
}
