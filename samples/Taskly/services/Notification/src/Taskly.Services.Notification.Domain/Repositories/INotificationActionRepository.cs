namespace Taskly.Services.Notification.Domain.Repositories;

public interface INotificationActionRepository
{
    Task<Entities.NotificationAction?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.NotificationAction notificationAction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.NotificationAction notificationAction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
