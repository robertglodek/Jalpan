using Taskly.Services.Identity.Domain.Entities;

namespace Taskly.Services.Identity.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}