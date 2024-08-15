namespace Jalpan.GatewayApi.Configuration;

public class OnError
{
    public int Code { get; set; } = 400;
    public object Data { get; set; }
}