using Jalpan.Types;
using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Documents;

internal sealed class NotificationDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Search { get; init; }
    public IEnumerable<string>? Tags { get; init; }
    public Guid? SectionId { get; init; }
    public Guid? GoalId { get; init; }
    public ScheduleDocument Schedule { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }

    public sealed class ScheduleDocument
    {
        public ReminderSchedule ReminderTiming { get; init; }
        public ScheduleFrequency ScheduleFrequency { get; init; }
        public int? CustomOffsetValue { get; init; }
        public TimeSpan SendTime { get; init; }
    }
}