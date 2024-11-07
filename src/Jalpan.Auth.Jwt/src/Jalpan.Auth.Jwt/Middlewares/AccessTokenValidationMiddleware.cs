using Jalpan.Auth.Jwt.Exceptions;
using Jalpan.Auth.Jwt.Managers;
using Microsoft.AspNetCore.Authorization;

namespace Jalpan.Auth.Jwt.Middlewares;

internal sealed class AccessTokenValidationMiddleware(IAccessTokenManager accessTokenManager) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null && RequiresAuthorization(endpoint))
        {
            if (await accessTokenManager.IsCurrentActiveToken())
            {
                await next(context);
            }
            else
            {
                throw new AccessTokenException("Access token is no longer valid as has been revoked.");
            }
        }
        else
        {
            await next(context);
        }
    }

    private static bool RequiresAuthorization(Endpoint endpoint) => endpoint.Metadata.Any(metadata => metadata is AuthorizeAttribute);
}