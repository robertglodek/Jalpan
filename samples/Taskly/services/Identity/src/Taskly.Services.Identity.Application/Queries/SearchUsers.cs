using Jalpan.Pagination;
using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Queries;

[UsedImplicitly]
public class SearchUsers: PagedQueryBase, IQuery<PagedResult<UserDto>>;