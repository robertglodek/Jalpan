using Taskly.Services.Notification.Domain.Exceptions;

namespace Taskly.Services.Notification.Application.Exceptions;

public sealed class InvalidNotificationNameException() : DomainException("Invalid notification name.")
{
    public override string Code => "invalid_notification_name";
}
