using System.Security.Claims;

namespace Jalpan.GatewayApi.Auth;

public interface IAuthorizationManager
{
    bool IsAuthorized(ClaimsPrincipal user, RouteConfig routeConfig);
}