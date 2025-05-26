namespace Taskly.Services.Task.Application.Exceptions;

public class UnauthorizedTaskAccessException(Guid taskId, Guid userId)
    : AppException($"Unauthorized access to task: '{taskId}' by user: '{userId}'")
{
    public override string Code => "unauthorized_task_access";
}
