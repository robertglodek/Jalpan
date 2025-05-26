using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Task.Application.DTO;
using Taskly.Services.Task.Application.Queries;
using Taskly.Services.Task.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Task.Infrastructure.Queries;

[UsedImplicitly]
internal sealed class GetTaskHandler(
IMongoDbRepository<TaskDocument, Guid> taskRepository,
IContextProvider contextProvider) : IQueryHandler<GetTask, TaskDetailsDto>
{
    public async Task<TaskDetailsDto?> HandleAsync(GetTask query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var task = await taskRepository.GetAsync(n => n.UserId == Guid.Parse(context.UserId!) && n.Id == query.Id,
            cancellationToken);

        return task?.AsDetailsDto();
    }
}
