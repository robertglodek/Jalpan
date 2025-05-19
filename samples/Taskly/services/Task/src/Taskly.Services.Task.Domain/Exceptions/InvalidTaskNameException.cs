namespace Taskly.Services.Task.Domain.Exceptions;

public sealed class InvalidTaskNameException() : DomainException("Task name cannot be null or empty.")
{
    public override string Code => "invalid_task_name";
}
