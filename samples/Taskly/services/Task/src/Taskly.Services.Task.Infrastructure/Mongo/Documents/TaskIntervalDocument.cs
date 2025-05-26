using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Infrastructure.Mongo.Documents;

internal sealed class TaskIntervalDocument
{
    public Interval Interval { get; init; }

    public DateTime? StartDate { get; init; }

    public TimeSpan? Hour { get; init; }
}
