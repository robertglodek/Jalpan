using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Jalpan.GatewayApi.Auth;

internal sealed class AuthenticationManager : IAuthenticationManager
{
    private readonly GatewayOptions _options;

    public AuthenticationManager(IOptions<GatewayOptions> options)
    {
        _options = options.Value;
    }

    public async Task<bool> TryAuthenticateAsync(HttpRequest request, RouteConfig routeConfig)
    {
        if (_options.Auth is null || !_options.Auth.Enabled || _options.Auth?.Global != true &&
            routeConfig.Route?.Auth != true)
        {
            return true;
        }

        var result = await request.HttpContext.AuthenticateAsync();

        return result.Succeeded;
    }
}