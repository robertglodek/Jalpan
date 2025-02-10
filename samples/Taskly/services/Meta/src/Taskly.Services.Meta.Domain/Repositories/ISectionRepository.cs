using Taskly.Services.Meta.Domain.Entities;

namespace Taskly.Services.Meta.Domain.Repositories;

public interface ISectionRepository
{
    Task<Section?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Section section, CancellationToken cancellationToken = default);
    Task UpdateAsync(Section section, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}