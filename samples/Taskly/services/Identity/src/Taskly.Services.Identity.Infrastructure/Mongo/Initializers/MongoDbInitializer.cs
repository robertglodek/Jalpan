using Jalpan;
using Jalpan.Persistence.MongoDB;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Initializers;

internal sealed class MongoDbInitializer(IMongoDbRepository<UserDocument, Guid> usersRepository) : IInitializer
{
    public async Task InitializeAsync()
    {
        await InitializeUsersAsync();
    }

    private async Task InitializeUsersAsync()
    {
        await usersRepository.Collection.CreateIndexAsync(true, u => u.Email);
    }
}