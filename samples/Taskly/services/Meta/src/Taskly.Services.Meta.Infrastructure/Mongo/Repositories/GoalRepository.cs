using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Repositories;

internal sealed class GoalRepository(IMongoDbRepository<GoalDocument, Guid> repository) : IGoalRepository
{

}