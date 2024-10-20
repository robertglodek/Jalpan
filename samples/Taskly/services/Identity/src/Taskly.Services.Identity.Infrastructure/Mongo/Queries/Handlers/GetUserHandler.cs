using Jalpan.Persistance.MongoDB.Repositories;
using Taskly.Services.Identity.Application.DTO;
using Taskly.Services.Identity.Application.Queries;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Queries.Handlers;

internal sealed  class GetUserHandler(IMongoDbRepository<UserDocument, Guid> userRepository) : IQueryHandler<GetUser, UserDto>
{
    private readonly IMongoDbRepository<UserDocument, Guid> _userRepository = userRepository;

    public async Task<UserDto?> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(query.UserId, cancellationToken);

        return user?.AsDto();
    }
}