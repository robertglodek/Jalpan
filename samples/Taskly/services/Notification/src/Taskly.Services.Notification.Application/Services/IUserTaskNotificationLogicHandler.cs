namespace Taskly.Services.Notification.Application.Services;

public interface IUserTaskNotificationLogicHandler
{
    Task HandleAsync(Guid notificationId);
}
