using Taskly.Services.Task.Domain.Enums;
using Taskly.Services.Task.Domain.Exceptions;

namespace Taskly.Services.Task.Domain.Entities;

public sealed class Task : AggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid? SectionId { get; private set; }
    public Guid? GoalId { get; private set; }
    public Guid? RootTaskId { get; private set; }
    public PriorityLevel PriorityLevel { get; private set; }
    public IEnumerable<Guid> Tags { get; private set; }
    public bool Repeatable { get; private set; }
    public bool Active { get; private set; }
    public TaskInterval? Interval { get; private set; }
    public DateTime? DueDate { get; private set; }

    public Task(
        string name,
        string? description,
        Guid? sectionId,
        Guid? goalId,
        Guid? rootTaskId,
        PriorityLevel priorityLevel,
        IEnumerable<Guid> tags,
        bool repeatable,
        bool active,
        TaskInterval? interval,
        DateTime? dueDate)
    {
        ValidateName(name);
        ValidateIntervalAndDueDate(interval, dueDate);

        Name = name;
        Description = description;
        SectionId = sectionId;
        GoalId = goalId;
        RootTaskId = rootTaskId;
        PriorityLevel = priorityLevel;
        Tags = tags;
        Repeatable = repeatable;
        Interval = interval;
        DueDate = dueDate;
        Active = interval is not null || active;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTaskNameException();
        }
    }

    private static void ValidateIntervalAndDueDate(TaskInterval? interval, DateTime? dueDate)
    {
        if (interval is not null && dueDate is not null)
        {
            throw new TaskCannotHaveIntervalAndDueDateException();
        }
    }
}