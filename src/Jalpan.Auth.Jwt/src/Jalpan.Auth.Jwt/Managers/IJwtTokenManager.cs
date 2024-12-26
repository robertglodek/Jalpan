namespace Jalpan.Auth.Jwt.Managers;

public interface IJwtTokenManager
{
    JsonWebToken CreateToken(
        string userId,
        string? email = null,
        string? role = null,
        IDictionary<string, IEnumerable<string>>? claims = null);
}
