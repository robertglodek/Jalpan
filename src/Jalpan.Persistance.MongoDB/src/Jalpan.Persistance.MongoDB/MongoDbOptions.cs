namespace Jalpan.Persistance.MongoDB;

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
    public bool Seed { get; set; }
}
