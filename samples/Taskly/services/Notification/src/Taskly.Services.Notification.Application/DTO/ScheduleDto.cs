using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Application.DTO;

public sealed class ScheduleDto
{
    public ReminderSchedule ReminderTiming { get; init; }
    public ScheduleFrequency ScheduleFrequency { get; init; }
    public int? CustomOffsetValue { get; init; }
    public TimeSpan SendTime { get; init; }
}
