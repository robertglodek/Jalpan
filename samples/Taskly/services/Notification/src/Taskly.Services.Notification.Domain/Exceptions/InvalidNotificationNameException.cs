namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidNotificationNameException() : DomainException("Invalid notification name.")
{
    public override string Code => "invalid_notification_name";
}
