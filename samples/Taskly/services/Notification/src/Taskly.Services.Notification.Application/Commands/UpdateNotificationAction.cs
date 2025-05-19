using Jalpan;
using Jalpan.Types;
using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Application.Commands;

public sealed class UpdateNotificationAction(
    Guid taskId,
    DateTime? notificationTime,
    UpdateNotificationAction.UpdateSchedule? schedule)
    : ICommand<Empty>
{
    public Guid TaskId { get; } = taskId;
    public UpdateSchedule? Schedule { get; } = schedule;
    public DateTime? NotificationTime { get; set; } = notificationTime;

    public sealed class UpdateSchedule(ReminderSchedule reminderSchedule, int? customOffsetValue, TimeSpan sendTime)
    {
        public ReminderSchedule ReminderTiming { get; } = reminderSchedule;
        public int? CustomOffsetValue { get; } = customOffsetValue;
        public TimeSpan SendTime { get; } = sendTime;
    }
}
