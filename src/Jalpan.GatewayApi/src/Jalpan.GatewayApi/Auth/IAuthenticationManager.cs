using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi.Auth;

public interface IAuthenticationManager
{
    Task<bool> TryAuthenticateAsync(HttpRequest request, RouteConfig routeConfig);
}