using Jalpan.Types;
using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Infrastructure.Mongo.Documents;

internal sealed class TaskDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public Guid? SectionId { get; init; }

    public Guid? GoalId { get; init; }

    public Guid? RootTaskId { get; init; }

    public PriorityLevel PriorityLevel { get; init; }

    public IEnumerable<Guid> Tags { get; init; } = null!;

    public bool Repeatable { get; init; }

    public bool Active { get; init; }

    public TaskIntervalDocument? Interval { get; init; }

    public DateTime? DueDate { get; init; }
}
