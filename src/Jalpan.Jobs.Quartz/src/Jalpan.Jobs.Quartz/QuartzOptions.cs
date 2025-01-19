namespace Jalpan.Jobs.Quartz;

public sealed class QuartzOptions
{
    public Dictionary<string, string> Schedules { get; init; } = [];
    public PostgresPersistencyOptions? PostgresPersistency { get; init; }
    public SqlServerPersistencyOptions? SqlServerPersistency { get; init; }

    public sealed class PostgresPersistencyOptions
    {
        public bool Enabled { get; init; }
        public string ConnectionString { get; init; } = string.Empty;
    }
    
    public sealed class SqlServerPersistencyOptions
    {
        public bool Enabled { get; init; }
        public string ConnectionString { get; init; } = string.Empty;
    }
}
