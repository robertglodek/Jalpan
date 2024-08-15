using Jalpan.GatewayApi.Configuration;

namespace Jalpan.GatewayApi;

internal interface IUpstreamBuilder
{
    string Build(Module module, Route route);
}