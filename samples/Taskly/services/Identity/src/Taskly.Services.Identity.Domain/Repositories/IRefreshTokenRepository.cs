using Taskly.Services.Identity.Domain.Entities;

namespace Taskly.Services.Identity.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
}