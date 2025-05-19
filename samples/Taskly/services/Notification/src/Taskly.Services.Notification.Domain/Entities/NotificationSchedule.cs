using Taskly.Services.Notification.Domain.Exceptions;

namespace Taskly.Services.Notification.Domain.Entities;

public sealed class NotificationSchedule : AggregateRoot
{
    public Guid UserId { get; set; }
    public string Name { get; private set; }
    public string? Search { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? GoalId { get; set; }
    public Schedule Schedule { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public NotificationSchedule(Guid id, DateTime createdAt, DateTime? lastModifiedAt, Guid userId, string name, string? search, IEnumerable<string>? tags, Guid? sectionId, Guid? goalId, Schedule schedule)
    {
        Id = id;

        if (Guid.Empty == userId)
        {
            throw new InvalidUserIdException();
        }

        UserId = userId;

        UpdateName(name);

        Search = search;
        Tags = tags;
        Schedule = schedule;
        SectionId = sectionId;
        GoalId = goalId;
        CreatedAt = createdAt;
        LastModifiedAt = lastModifiedAt;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidNotificationNameException();
        }
        Name = name;
    }
}
