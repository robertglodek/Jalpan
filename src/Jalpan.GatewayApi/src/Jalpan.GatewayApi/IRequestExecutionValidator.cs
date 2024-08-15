using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi;

internal interface IRequestExecutionValidator
{
    Task<bool> TryExecuteAsync(HttpContext context, RouteConfig routeConfig);
}