namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidPermissionException() : DomainException($"Invalid permission.")
{
    public override string Code => "invalid_permission";
}
