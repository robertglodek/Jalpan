using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Application.DTO;

public sealed class TaskDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? GoalId { get; set; }
    public Guid? RootTaskId { get; set; }
    public PriorityLevel PriorityLevel { get; set; }
    public IEnumerable<Guid> Tags { get; set; } = null!;
    public bool Repeatable { get; set; }
    public bool Active { get; set; }
    public TaskIntervalDto? Interval { get; set; }
    public DateTime? DueDate { get; set; }
}
