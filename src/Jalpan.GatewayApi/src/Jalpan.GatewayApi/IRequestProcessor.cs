using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi;

public interface IRequestProcessor
{
    Task<ExecutionData> ProcessAsync(RouteConfig routeConfig, HttpContext context);
}