namespace Taskly.Services.Notification.Domain.Repositories;

public interface IPermissionAssignmentRepository
{
    Task<Entities.PermissionAssignment?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.PermissionAssignment permissionAssignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.PermissionAssignment permissionAssignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
