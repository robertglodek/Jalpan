using MongoDB.Driver;

namespace Jalpan.Persistance.MongoDB.Factories;

internal sealed class MongoDbSessionFactory : IMongoDbSessionFactory
{
    private readonly IMongoClient _client;

    public MongoDbSessionFactory(IMongoClient client)
        => _client = client;

    public Task<IClientSessionHandle> CreateAsync()
        => _client.StartSessionAsync();
}
