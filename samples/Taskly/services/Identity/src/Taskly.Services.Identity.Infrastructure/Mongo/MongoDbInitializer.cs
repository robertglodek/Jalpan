using Jalpan;
using Jalpan.Persistence.MongoDB.Repositories;
using MongoDB.Driver;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo;

internal sealed class MongoDbInitializer(IMongoDbRepository<UserDocument, Guid> usersRepository) : IInitializer
{
    private IMongoDbRepository<UserDocument, Guid> UsersRepository { get; } = usersRepository;

    public async Task InitializeAsync()
    {
        await InitializeUsersAsync();
    }

    private async Task InitializeUsersAsync()
    {
        var indexKeysDefinition = Builders<UserDocument>.IndexKeys.Ascending(u => u.Email);

        var indexModel = new CreateIndexModel<UserDocument>(indexKeysDefinition, new CreateIndexOptions { Unique = true });

        await UsersRepository.Collection.Indexes.CreateOneAsync(indexModel);
    }
}
