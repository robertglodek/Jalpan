using Taskly.Services.Task.Domain.Enums;
using Taskly.Services.Task.Domain.Exceptions;

namespace Taskly.Services.Task.Domain.Entities;

public sealed class TaskInterval
{
    public Interval Interval { get; private set; }
    public DateTime? StartDate { get; private set; }
    public TimeSpan? Hour { get; private set; }

    private TaskInterval() { }

    public TaskInterval(Interval interval, DateTime? startDate, TimeSpan? hour)
    {
        ValidateInterval(interval, startDate);
        Interval = interval;
        StartDate = startDate;
        Hour = hour;
    }

    private static void ValidateInterval(Interval interval, DateTime? startDate)
    {
        switch (interval)
        {
            case Interval.Every2Days:
            case Interval.Every3Days:
            case Interval.Every4Days:
            case Interval.Every5Days:
            case Interval.Every6Days:
            case Interval.Every7Days:
            case Interval.EveryWeek:
            case Interval.EveryMonth:
            case Interval.EveryYear:
                if (!startDate.HasValue)
                {
                    throw new MissingIntervalStartDateException();
                }
                break;
        }
    }
}
