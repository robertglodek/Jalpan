namespace Taskly.Services.Notification.Domain.Repositories;

public interface IRoleAssignmentRepository
{
    Task<Entities.RoleAssignment?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.RoleAssignment roleAssignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.RoleAssignment roleAssignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
