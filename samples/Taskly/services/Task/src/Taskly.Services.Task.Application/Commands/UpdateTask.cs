using Jalpan;
using Jalpan.Types;
using JetBrains.Annotations;
using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Application.Commands;

[UsedImplicitly]
public sealed class UpdateTask : ICommand<Empty>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; }
    public Guid? SectionId { get; }
    public Guid? GoalId { get; }
    public Guid? RootTaskId { get; }
    public PriorityLevel PriorityLevel { get; }
    public IEnumerable<Guid>? Tags { get; }
    public bool Repeatable { get; }
    public bool Active { get; }
    public UpdateTaskInterval? Interval { get; }
    public DateTime? DueDate { get; }

    public sealed class UpdateTaskInterval
    {
        public Interval Interval { get; private set; }
        public DateTime? StartDate { get; private set; }
        public TimeSpan? Hour { get; private set; }
    }
}

