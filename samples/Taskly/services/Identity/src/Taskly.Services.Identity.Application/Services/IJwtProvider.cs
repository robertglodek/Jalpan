using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Services;

public interface IJwtProvider
{
    AccessTokenDto Create(Guid userId, string email, string role,
        IDictionary<string, IEnumerable<string>>? claims = null);
}