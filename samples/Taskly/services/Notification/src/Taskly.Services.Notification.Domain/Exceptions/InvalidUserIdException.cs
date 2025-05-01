namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidUserIdException() : DomainException("Invalid user identifier")
{
    public override string Code => "invalid_user_identifier";
}
