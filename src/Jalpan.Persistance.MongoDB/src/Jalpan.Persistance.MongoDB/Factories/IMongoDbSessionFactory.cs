using MongoDB.Driver;

namespace Jalpan.Persistance.MongoDB.Factories;

public interface IMongoDbSessionFactory
{
    Task<IClientSessionHandle> CreateAsync();
}
