namespace Taskly.Services.Notification.Domain.Repositories;

public interface INotificationRepository
{
    Task<Entities.Notification?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
