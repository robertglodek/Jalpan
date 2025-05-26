using Jalpan.Types;
using JetBrains.Annotations;
using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Application.Commands;

[UsedImplicitly]
public sealed class AddTask(
        string name,
        string? description,
        Guid? sectionId,
        Guid? goalId,
        Guid? rootTaskId,
        PriorityLevel priorityLevel,
        IEnumerable<Guid> tags,
        bool repeatable,
        bool active,
        AddTask.AddTaskInterval? interval,
        DateTime? dueDate) : ICommand<Guid>
{
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public Guid? SectionId { get; } = sectionId;
    public Guid? GoalId { get; } = goalId;
    public Guid? RootTaskId { get; } = rootTaskId;
    public PriorityLevel PriorityLevel { get; } = priorityLevel;
    public IEnumerable<Guid> Tags { get; } = tags;
    public bool Repeatable { get; } = repeatable;
    public bool Active { get; } = active;
    public AddTaskInterval? Interval { get; } = interval;
    public DateTime? DueDate { get; } = dueDate;

    public sealed class AddTaskInterval(
        Interval interval,
        DateTime? startDate,
        TimeSpan? hour)
    {
        public Interval Interval { get; private set; } = interval;
        public DateTime? StartDate { get; private set; } = startDate;
        public TimeSpan? Hour { get; private set; } = hour;
    }
}
