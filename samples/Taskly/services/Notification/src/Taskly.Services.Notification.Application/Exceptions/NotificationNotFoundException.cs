namespace Taskly.Services.Notification.Application.Exceptions;

public sealed class NotificationNotFoundException(Guid notificationId) : AppException($"Notification with ID: '{notificationId}' was not found.")
{
    public override string Code => "notification_not_found";
    public Guid NotificationId { get; } = notificationId;
}
