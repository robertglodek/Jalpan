namespace Jalpan.GatewayApi.Configuration;

public class LoadBalancer
{
    public bool Enabled { get; set; }
    public string Url { get; set; } = null!;
}