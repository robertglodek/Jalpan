using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jalpan.GatewayApi;

internal interface IDownstreamBuilder
{
    string GetDownstream(RouteConfig routeConfig, HttpRequest request, RouteData data);
}