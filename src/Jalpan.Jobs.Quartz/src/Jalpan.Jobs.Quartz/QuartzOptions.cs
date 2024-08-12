namespace Jalpan.Jobs.Quartz;

public class QuartzOptions
{
    /// <summary>
    /// Dictionary containing jobs' names and its' cron schedules
    /// </summary>
    public Dictionary<string, string> Schedules { get; set; } = [];
}
