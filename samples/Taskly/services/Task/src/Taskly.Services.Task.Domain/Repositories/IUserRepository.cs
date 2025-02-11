namespace Taskly.Services.Task.Domain.Repositories;

public interface IUserRepository
{
    Task<Entities.Task?> GetAsync(Guid id);
    Task<Entities.Task?> GetAsync(string email);
    System.Threading.Tasks.Task AddAsync(Entities.Task task);
    System.Threading.Tasks.Task UpdateAsync(Entities.Task task);
}