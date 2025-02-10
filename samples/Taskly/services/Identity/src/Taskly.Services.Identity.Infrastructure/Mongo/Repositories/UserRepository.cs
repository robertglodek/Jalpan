using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Identity.Domain.Entities;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

internal sealed class UserRepository(IMongoDbRepository<UserDocument, Guid> repository) : IUserRepository
{
    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetAsync(id, cancellationToken);
        return user?.AsEntity();
    }

    public async Task<User?> GetAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetAsync(x => x.Email == email.ToLowerInvariant(), cancellationToken);
        return user?.AsEntity();
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        repository.AddAsync(user.AsDocument(), cancellationToken: cancellationToken);

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default) =>
        repository.UpdateAsync(user.AsDocument(), cancellationToken);
}