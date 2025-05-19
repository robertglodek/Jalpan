using MongoDB.Driver;

namespace Jalpan.Persistence.MongoDB.Migrations;

public abstract class MigrationBase
{
    public abstract string Name { get; }
    public abstract DateTime Timestamp { get; }
    public abstract Task Up(IMongoDatabase db);
}
