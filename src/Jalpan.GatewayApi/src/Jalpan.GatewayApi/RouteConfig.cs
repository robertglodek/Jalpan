using Jalpan.GatewayApi.Configuration;

namespace Jalpan.GatewayApi;

public class RouteConfig
{
    public Route Route { get; set; } = null!;
    public string Downstream { get; set; } = null!;
}