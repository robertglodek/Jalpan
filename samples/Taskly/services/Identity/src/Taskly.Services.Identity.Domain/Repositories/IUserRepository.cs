using Taskly.Services.Identity.Core.Entities;

namespace Taskly.Services.Identity.Core.Exceptions;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id);
    Task<User?> GetAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}