namespace Jalpan.GatewayApi.WebApi;

public class WebApiEndpointDefinition
{
    public string Method { get; set; }
    public string Path { get; set; }
    public IEnumerable<WebApiEndpointParameter> Parameters { get; set; } = [];
    public IEnumerable<WebApiEndpointResponse> Responses { get; set; } = [];
}