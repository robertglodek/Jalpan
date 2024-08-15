using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jalpan.GatewayApi;

internal interface IPayloadTransformer
{
    bool HasTransformations(string resourceId, Configuration.Route route);
    PayloadSchema Transform(string payload, string resourceId, Configuration.Route route, HttpRequest request, RouteData data);
}