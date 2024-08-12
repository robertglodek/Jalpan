using MongoDB.Driver;

namespace Jalpan.Persistance.MongoDB.Factories;

public interface IMongoSessionFactory
{
    Task<IClientSessionHandle> CreateAsync();
}
