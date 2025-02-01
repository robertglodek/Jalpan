using Jalpan;
using Jalpan.Persistence.MongoDB.Repositories;
using MongoDB.Driver;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Initializers;

internal sealed class MongoDbInitializer(IMongoDbRepository<GoalDocument, Guid> usersRepository) : IInitializer
{
    private IMongoDbRepository<GoalDocument, Guid> UsersRepository { get; } = usersRepository;

    public async Task InitializeAsync()
    {
        await InitializeUsersAsync();
    }

    private async Task InitializeUsersAsync()
    {
        var indexKeysDefinition = Builders<GoalDocument>.IndexKeys.Ascending(u => u.Email);

        var indexModel =
            new CreateIndexModel<GoalDocument>(indexKeysDefinition, new CreateIndexOptions { Unique = true });

        await UsersRepository.Collection.Indexes.CreateOneAsync(indexModel);
    }
}