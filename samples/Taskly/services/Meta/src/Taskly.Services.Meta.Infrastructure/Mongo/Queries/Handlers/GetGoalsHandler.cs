using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetGoalsHandler(
    IMongoDbRepository<GoalDocument, Guid> goalRepository,
    IContextProvider contextProvider) : IQueryHandler<GetGoals, IEnumerable<GoalDto>>
{
    public async Task<IEnumerable<GoalDto>?> HandleAsync(GetGoals query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        var users = await goalRepository.FindAsync(n => n.UserId == Guid.Parse(context.UserId!), cancellationToken);

        return users.Select(n => n.AsDto());
    }
}