using Taskly.Services.Notification.Domain.Entities;
using Taskly.Services.Notification.Domain.Repositories;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Repositories;

internal sealed class RoleAssignmentRepository : IRoleAssignmentRepository
{
    public Task AddAsync(RoleAssignment roleAssignment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RoleAssignment?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(RoleAssignment roleAssignment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
