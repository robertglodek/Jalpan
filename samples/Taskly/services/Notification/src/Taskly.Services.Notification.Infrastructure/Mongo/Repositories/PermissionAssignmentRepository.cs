using Taskly.Services.Notification.Domain.Entities;
using Taskly.Services.Notification.Domain.Repositories;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Repositories;

internal sealed class PermissionAssignmentRepository : IPermissionAssignmentRepository
{
    public Task AddAsync(PermissionAssignment permissionAssignment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PermissionAssignment?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(PermissionAssignment permissionAssignment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
