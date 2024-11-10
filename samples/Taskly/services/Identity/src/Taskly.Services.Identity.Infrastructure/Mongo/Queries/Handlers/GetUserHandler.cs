using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Identity.Application.DTO;
using Taskly.Services.Identity.Application.Queries;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Queries.Handlers;

internal sealed  class GetUserHandler(IMongoDbRepository<UserDocument, Guid> userRepository) : IQueryHandler<GetUser, UserDto>
{
    public async Task<UserDto?> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(query.UserId, cancellationToken);

        return user?.AsDto();
    }
}