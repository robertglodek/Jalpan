using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Domain.Entities;
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Repositories;

internal sealed class GoalRepository(IMongoDbRepository<GoalDocument, Guid> repository) : IGoalRepository
{
    public async Task<Goal?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var goal = await repository.GetAsync(id, cancellationToken);
        return goal?.AsEntity();
    }

    public async Task AddAsync(Goal goal, CancellationToken cancellationToken = default)
        => await repository.AddAsync(goal.AsDocument(), cancellationToken: cancellationToken);

    public async Task UpdateAsync(Goal goal, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(goal.AsDocument(), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken: cancellationToken);
}