using Microsoft.AspNetCore.Routing;

namespace Jalpan.GatewayApi;

internal interface IRouteProvider
{
    Action<IEndpointRouteBuilder> Build();
}