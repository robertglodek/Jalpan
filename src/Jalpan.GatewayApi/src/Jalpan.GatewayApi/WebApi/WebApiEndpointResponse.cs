namespace Jalpan.GatewayApi.WebApi;

public class WebApiEndpointResponse
{
    public string Type { get; set; }
    public int StatusCode { get; set; }
    public object Example { get; set; }
}