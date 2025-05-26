using Taskly.Services.Task.Domain.Enums;
using Taskly.Services.Task.Domain.Exceptions;

namespace Taskly.Services.Task.Domain.Entities;

public sealed class Task : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? GoalId { get; set; }
    public Guid? RootTaskId { get; set; }
    public PriorityLevel PriorityLevel { get; private set; }
    public IEnumerable<Guid> Tags { get; private set; }
    public bool Repeatable { get; private set; }
    public bool Active { get; private set; }
    public TaskInterval? Interval { get; private set; }
    public DateTime? DueDate { get; private set; }

    public Task(
        Guid id,
        Guid userId,
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
        UpdateName(name);
        ValidateIntervalAndDueDate(interval, dueDate);

        Id = id;
        UserId = userId;
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

    public void UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new InvalidTaskNameException();
        Name = name;
    }

    private static void ValidateIntervalAndDueDate(TaskInterval? interval, DateTime? dueDate)
    {
        if (interval is not null && dueDate is not null)
        {
            throw new TaskCannotHaveIntervalAndDueDateException();
        }
    }
}