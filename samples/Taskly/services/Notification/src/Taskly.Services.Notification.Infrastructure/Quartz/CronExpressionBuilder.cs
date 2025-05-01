using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Infrastructure.Quartz;

public static class CronExpressionBuilder
{
    /// <summary>
    /// Generates a CRON expression based on frequency and time.
    /// </summary>
    /// <param name="frequency">The frequency of the job.</param>
    /// <param name="time">The time of day when the job should run.</param>
    /// <returns>A CRON expression string compatible with Quartz.NET.</returns>
    public static string Generate(ScheduleFrequency scheduleFrequency, TimeSpan sendTime)
    {
        int hour = sendTime.Hours;
        int minute = sendTime.Minutes;

        return scheduleFrequency switch
        {
            ScheduleFrequency.Daily => $"{minute} {hour} * * ?",
            ScheduleFrequency.Every2Days => $"{minute} {hour} 1/2 * ?",
            ScheduleFrequency.Every3Days => $"{minute} {hour} 1/3 * ?",
            ScheduleFrequency.Weekly => $"{minute} {hour} ? * 1",  // Monday
            ScheduleFrequency.Monthly => $"{minute} {hour} 1 * ?",
            _ => throw new ArgumentOutOfRangeException(nameof(scheduleFrequency), scheduleFrequency, null)
        };
    }
}