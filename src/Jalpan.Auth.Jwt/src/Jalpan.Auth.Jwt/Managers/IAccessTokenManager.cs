namespace Jalpan.Auth.Jwt.Managers;

public interface IAccessTokenManager
{
    Task<bool> IsCurrentActiveToken();
    Task DeactivateCurrentAsync();
    Task<bool> IsActiveAsync(string token);
    Task DeactivateAsync(string token);
}
