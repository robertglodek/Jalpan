using Jalpan.Types;
using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Application.Commands;

public sealed class CreateNotificationAction(
    Guid taskId,
    DateTime? notificationTime,
    CreateNotificationAction.CreateSchedule? schedule)
    : ICommand<Guid>
{
    public Guid TaskId { get; } = taskId;
    public CreateSchedule? Schedule { get; } = schedule;
    public DateTime? NotificationTime { get; set; } = notificationTime;

    public sealed class CreateSchedule(ReminderSchedule reminderSchedule, int? customOffsetValue, TimeSpan sendTime)
    {
        public ReminderSchedule ReminderTiming { get; } = reminderSchedule;
        public int? CustomOffsetValue { get; } = customOffsetValue;
        public TimeSpan SendTime { get; } = sendTime;
    }
}