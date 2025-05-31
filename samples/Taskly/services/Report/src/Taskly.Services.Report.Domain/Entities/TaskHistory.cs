namespace Taskly.Services.Report.Domain.Entities;

public sealed class TaskHistory : AggregateRoot
{
    public Guid UserId { get; set; }
    public Guid TaskId { get; private set; }
    public string TaskName { get; private set; }
    public bool Success { get; private set; }
    public DateTime DateTime { get; private set; }
    public TaskHistory(Guid id, Guid userId, Guid taskId, string taskName, bool success, DateTime dateTime, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        TaskId = taskId;
        TaskName = taskName;
        Success = success;
        DateTime = dateTime;
        CreatedAt = createdAt;
    }
}
