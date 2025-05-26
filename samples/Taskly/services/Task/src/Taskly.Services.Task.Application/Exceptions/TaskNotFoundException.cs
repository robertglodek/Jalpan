namespace Taskly.Services.Task.Application.Exceptions;

public sealed class TaskNotFoundException(Guid taskId) : AppException($"Task with ID: '{taskId}' was not found.")
{
    public override string Code => "task_not_found";
}
