using Taskly.Services.Meta.Domain.Entities;

namespace Taskly.Services.Meta.Domain.Repositories;

public interface ISectionRepository
{
    Task<Section?> GetAsync(Guid id);
    Task AddAsync(Section section);
    Task UpdateAsync(Section section);
    Task DeleteAsync(Guid id);
}