namespace Taskly.Services.Task.Domain.Exceptions;

public sealed class TaskCannotHaveIntervalAndDueDateException() : DomainException("A task cannot have both an interval and a due date.")
{
    public override string Code => "task_conflicting_interval_due_date";
}
