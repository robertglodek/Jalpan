namespace Jalpan.GatewayApi.WebApi;

public class WebApiEndpointDefinition
{
    public string Method { get; set; } = null!;
    public string Path { get; set; } = null!;
    public IEnumerable<WebApiEndpointParameter> Parameters { get; set; } = [];
    public IEnumerable<WebApiEndpointResponse> Responses { get; set; } = [];
}