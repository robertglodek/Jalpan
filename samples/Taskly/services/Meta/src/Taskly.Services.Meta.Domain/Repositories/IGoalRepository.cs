using Taskly.Services.Meta.Domain.Entities;

namespace Taskly.Services.Meta.Domain.Repositories;

public interface IGoalRepository
{
    Task<Goal?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Goal goal, CancellationToken cancellationToken = default);
    Task UpdateAsync(Goal goal, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}