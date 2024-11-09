namespace Jalpan.Persistence.Redis;

public sealed class RedisOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string? Instance { get; init; }
    public int Database { get; init; }
}
