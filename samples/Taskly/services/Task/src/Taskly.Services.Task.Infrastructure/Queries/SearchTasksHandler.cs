using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Pagination;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Task.Application.DTO;
using Taskly.Services.Task.Application.Queries;
using Taskly.Services.Task.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Task.Infrastructure.Queries;

[UsedImplicitly]
internal sealed class SearchTasksHandler(
    IMongoDbRepository<TaskDocument, Guid> taskRepository,
    IContextProvider contextProvider) : IQueryHandler<SearchTasks, PagedResult<TaskDto>>
{
    public async Task<PagedResult<TaskDto>?> HandleAsync(SearchTasks query,
        CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        PagedResult<TaskDocument> pagedResult;
        pagedResult = await taskRepository.BrowseAsync(n => n.UserId == Guid.Parse(context.UserId!), query,
                cancellationToken);

        return pagedResult.Map(d => d.AsDto());
    }
}
