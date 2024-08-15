using Jalpan.GatewayApi.Configuration;

namespace Jalpan.GatewayApi;

internal interface IRouteConfigurator
{
    RouteConfig Configure(Module module, Route route);
}