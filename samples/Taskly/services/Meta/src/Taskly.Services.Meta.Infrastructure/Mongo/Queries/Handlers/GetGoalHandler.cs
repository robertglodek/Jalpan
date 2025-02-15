using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetGoalHandler(
    IMongoDbRepository<GoalDocument, Guid> goalRepository,
    IContextProvider contextProvider) : IQueryHandler<GetGoal, GoalDto>
{
    public async Task<GoalDto?> HandleAsync(GetGoal query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var goal = await goalRepository.GetAsync(n => n.UserId == Guid.Parse(context.UserId!) && n.Id == query.Id,
            cancellationToken);

        return goal?.AsDto();
    }
}