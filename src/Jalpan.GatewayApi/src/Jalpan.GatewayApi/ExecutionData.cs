using Jalpan.GatewayApi.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Dynamic;

namespace Jalpan.GatewayApi;

public class ExecutionData
{
    public string RequestId { get; set; }
    public string ResourceId { get; set; }
    public string TraceId { get; set; }
    public string UserId { get; set; }
    public IDictionary<string, string> Claims { get; set; }
    public string ContentType { get; set; }
    public Route Route { get; set; }
    public HttpContext Context { get; set; }
    public RouteData Data { get; set; }
    public string Downstream { get; set; }
    public ExpandoObject Payload { get; set; }
    public bool HasPayload { get; set; }
    public IEnumerable<Error> ValidationErrors { get; set; } = [];
    public bool IsPayloadValid => ValidationErrors is null || !ValidationErrors.Any();
}