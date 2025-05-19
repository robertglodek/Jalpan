namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidNotificationDateException() : DomainException("Invalid notification date.")
{
    public override string Code => "invalid_notification_date";
}
