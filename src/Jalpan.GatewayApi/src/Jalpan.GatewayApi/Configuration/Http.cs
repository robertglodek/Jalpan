namespace Jalpan.GatewayApi.Configuration;

public class Http
{
    public int Retries { get; set; } = 3;
    public TimeSpan? RetryInterval { get; set; }
    public bool Exponential { get; set; }
}