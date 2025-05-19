namespace Taskly.Services.Notification.Domain.Repositories;

public interface INotificationScheduleRepository
{
    Task<Entities.NotificationSchedule?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.NotificationSchedule notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.NotificationSchedule notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
