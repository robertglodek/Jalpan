namespace Jalpan.Jobs.Quartz;

public class QuartzOptions
{
    public Dictionary<string, string> Schedules { get; init; } = [];
    public PostgresPersistencyOptions? PostgresPersistency { get; init; }

    public class PostgresPersistencyOptions
    {
        public bool Enabled { get; init; }
        public string ConnectionString { get; init; } = string.Empty;
    }
}
