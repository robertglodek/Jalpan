using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Application.DTO;

public sealed class TaskDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public PriorityLevel PriorityLevel { get; set; }
    public bool Repeatable { get; set; }
    public bool Active { get; set; }
}
