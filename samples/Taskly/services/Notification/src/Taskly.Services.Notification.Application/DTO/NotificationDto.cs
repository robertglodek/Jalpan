namespace Taskly.Services.Notification.Application.DTO;

public sealed class NotificationDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public string? Search { get; init; }
    public IEnumerable<string>? Tags { get; init; }
    public Guid? SectionId { get; init; }
    public Guid? GoalId { get; init; }
}
