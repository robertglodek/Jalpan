using Jalpan.Handlers;
using Jalpan.Pagination;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class SearchUsersHandler(IMongoDbRepository<GoalDocument, Guid> repository)
    : IQueryHandler<SearchUsers, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>?> HandleAsync(SearchUsers query, CancellationToken cancellationToken = default)
    {
        var pagedResult = await repository.BrowseAsync(_ => true, query, cancellationToken);

        return pagedResult.Map(d => d.AsDto());
    }
}