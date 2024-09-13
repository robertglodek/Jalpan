using Jalpan.GatewayApi.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Dynamic;

namespace Jalpan.GatewayApi;

public class ExecutionData
{
    public string ActivityId { get; set; } = null!;
    public string? UserId { get; set; }
    public IDictionary<string, string> Claims { get; set; }
    public string ContentType { get; set; }
    public Configuration.Route Route { get; set; }
    public HttpContext Context { get; set; }
    public RouteData Data { get; set; }
    public string Downstream { get; set; } = null!;
    public ExpandoObject? Payload { get; set; }
    public bool HasPayload { get; set; }
}