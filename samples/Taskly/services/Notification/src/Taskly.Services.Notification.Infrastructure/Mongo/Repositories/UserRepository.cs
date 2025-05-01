using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Notification.Domain.Entities;
using Taskly.Services.Notification.Domain.Repositories;
using Taskly.Services.Notification.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Repositories;

internal sealed class UserRepository(IMongoDbRepository<UserDocument, Guid> repository) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await repository.AddAsync(user.AsDocument(), cancellationToken: cancellationToken);

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => (await repository.GetAsync(id, cancellationToken: cancellationToken))?.AsEntity();

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(user.AsDocument(), cancellationToken);
}
