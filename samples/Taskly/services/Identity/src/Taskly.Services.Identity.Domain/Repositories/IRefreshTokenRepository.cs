using Taskly.Services.Identity.Core.Entities;

namespace Taskly.Services.Identity.Core.Exceptions;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string token);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
}