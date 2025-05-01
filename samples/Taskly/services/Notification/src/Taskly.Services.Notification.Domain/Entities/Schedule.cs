using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Domain.Entities;

public sealed class Schedule(ReminderSchedule reminderTiming, ScheduleFrequency scheduleFrequency, int? customOffsetValue, TimeSpan sendTime)
{
    public ReminderSchedule ReminderTiming { get; set; } = reminderTiming;
    public int? CustomOffsetValue { get; set; } = customOffsetValue;
    public TimeSpan SendTime { get; set; } = sendTime;
    public ScheduleFrequency ScheduleFrequency { get; set; } = scheduleFrequency;

    public int GetDaysBefore(DateTime eventDate) => ReminderTiming switch
    {
        ReminderSchedule.SameDay => 0,
        ReminderSchedule.InOneDay => 1,
        ReminderSchedule.InTwoDays => 2,
        ReminderSchedule.InOneWeek => 7,
        ReminderSchedule.InOneMonth => (eventDate - eventDate.AddMonths(-1)).Days,
        ReminderSchedule.CustomDays => CustomOffsetValue ?? 0,
        _ => throw new ArgumentOutOfRangeException()
    };
}
