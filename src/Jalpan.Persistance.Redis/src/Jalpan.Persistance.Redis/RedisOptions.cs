namespace Jalpan.Persistance.Redis;

public class RedisOptions
{
    public string ConnectionString { get; set; } = null!;
    public string? Instance { get; set; }
    public int Database { get; set; }
}
