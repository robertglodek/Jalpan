namespace Jalpan.Auth.Services;

public interface IJwtTokenManager
{
    JsonWebToken CreateToken(
        string userId,
        string? role = null,
        string? audience = null,
        IDictionary<string, IEnumerable<string>>? claims = null);
}
