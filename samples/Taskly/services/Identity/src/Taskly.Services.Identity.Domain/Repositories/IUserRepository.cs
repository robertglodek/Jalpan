using Taskly.Services.Identity.Domain.Entities;

namespace Taskly.Services.Identity.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id);
    Task<User?> GetAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}