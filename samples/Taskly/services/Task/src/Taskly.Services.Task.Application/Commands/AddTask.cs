using Jalpan.Types;
using JetBrains.Annotations;
using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Application.Commands;

[UsedImplicitly]
public sealed class AddTask : ICommand<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; }
    public Guid? SectionId { get; }
    public Guid? GoalId { get; }
    public Guid? RootTaskId { get; }
    public PriorityLevel PriorityLevel { get; }
    public IEnumerable<Guid>? Tags { get; }
    public bool Repeatable { get; }
    public bool Active { get; }
    public AddTaskInterval? Interval { get; }
    public DateTime? DueDate { get; }

    public sealed class AddTaskInterval
    {
        public Interval Interval { get; private set; }
        public DateTime? StartDate { get; private set; }
        public TimeSpan? Hour { get; private set; }
    }
}
