namespace Taskly.Services.Note.Domain.Repositories;

public interface INoteRepository
{
    Task<Entities.Note?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Note note, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Note note, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}