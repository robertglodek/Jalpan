using MongoDB.Driver;

namespace Jalpan.Persistance.MongoDB.Seeders;

public interface IMongoDbSeeder
{
    Task SeedAsync(IMongoDatabase database);
}
