using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi;

public interface IRequestHandlerManager
{
    IHandler Get(string name);
    void AddHandler(string name, IHandler handler);
    Task HandleAsync(string handler, HttpContext context, RouteConfig routeConfig);
}