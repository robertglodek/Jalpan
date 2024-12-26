using Jalpan.Auth.Jwt.Managers;
using Jalpan.Time;
using Taskly.Services.Identity.Application.DTO;
using Taskly.Services.Identity.Application.Services;

namespace Taskly.Services.Identity.Infrastructure.Auth;

internal sealed class JwtProvider(IJwtTokenManager jwtTokenManager, IDateTime dateTime) : IJwtProvider
{
    public AccessTokenDto Create(Guid userId, string email, string role,
        IDictionary<string, IEnumerable<string>>? claims = null)
    {
        var jwt = jwtTokenManager.CreateToken(userId.ToString("N"), email, role, claims);
        return new AccessTokenDto
        {
            ExpiresAt = dateTime.UnixMsToDateTime(jwt.Expiry),
            Token = jwt.AccessToken
        };
    }
}