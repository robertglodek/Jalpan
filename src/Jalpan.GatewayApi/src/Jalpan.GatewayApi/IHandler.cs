using Jalpan.GatewayApi.Configuration;
using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi;

public interface IHandler
{
    string GetInfo(Route route);
    Task HandleAsync(HttpContext context, RouteConfig config);
}