namespace Jalpan.Jobs.Quartz;

public class QuartzOptions
{
    /// <summary>
    /// Dictionary containing jobs' names and its' cron schedules
    /// </summary>
    public Dictionary<string, string> Schedules { get; set; } = [];

    public PostgresPersistencyOptions? PostgresPersistency { get; set; }

    public class PostgresPersistencyOptions
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
    }
}
