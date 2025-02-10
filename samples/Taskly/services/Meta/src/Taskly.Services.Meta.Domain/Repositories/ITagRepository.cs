using Taskly.Services.Meta.Domain.Entities;

namespace Taskly.Services.Meta.Domain.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}