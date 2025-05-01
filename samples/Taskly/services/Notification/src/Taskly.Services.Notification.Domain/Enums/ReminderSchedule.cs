namespace Taskly.Services.Notification.Domain.Enums;

public enum ReminderSchedule : byte
{
    SameDay,
    InOneDay,
    InTwoDays,
    InOneWeek,
    InOneMonth,
    CustomDays // Allows user-defined offsets
}
