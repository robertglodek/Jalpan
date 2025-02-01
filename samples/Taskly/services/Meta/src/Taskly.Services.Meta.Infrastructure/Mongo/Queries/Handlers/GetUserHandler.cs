using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetUserHandler(IMongoDbRepository<GoalDocument, Guid> userRepository)
    : IQueryHandler<GetUser, UserDetailsDto>
{
    public async Task<UserDetailsDto?> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(query.UserId, cancellationToken);

        return user?.AsDetailsDto();
    }
}