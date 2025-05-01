using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Application.Services;

public interface IUserTaskNotificationJobManager
{
    Task ScheduleNotificationJobAsync(Guid notificationId, ScheduleFrequency scheduleFrequency, TimeSpan sendTime);
    Task RemoveNotificationJobAsync(Guid notificationId);
    Task RescheduleNotificationJobAsync(Guid notificationId, ScheduleFrequency scheduleFrequency, TimeSpan sendTime);
}
