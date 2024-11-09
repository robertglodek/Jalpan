namespace Jalpan.Persistence.MongoDB;

public sealed class MongoDbOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string Database { get; init; } = string.Empty;
}
