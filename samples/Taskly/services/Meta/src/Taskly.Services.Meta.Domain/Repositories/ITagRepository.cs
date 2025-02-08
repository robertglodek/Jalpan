using Taskly.Services.Meta.Domain.Entities;

namespace Taskly.Services.Meta.Domain.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetAsync(Guid id);
    Task AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(Guid id);
}