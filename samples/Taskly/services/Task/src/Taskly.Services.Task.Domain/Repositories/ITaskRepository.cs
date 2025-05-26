namespace Taskly.Services.Task.Domain.Repositories;

public interface ITaskRepository
{
    Task<Entities.Task?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task AddAsync(Entities.Task task, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task UpdateAsync(Entities.Task task, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}