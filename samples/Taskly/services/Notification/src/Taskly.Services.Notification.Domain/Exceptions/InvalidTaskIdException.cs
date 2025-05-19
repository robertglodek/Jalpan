namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidTaskIdException() : DomainException("Invalid task id.")
{
    public override string Code => "invalid_task_id";
}
