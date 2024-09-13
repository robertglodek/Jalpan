namespace Jalpan.GatewayApi.Configuration;

public class Module
{
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public bool? Enabled { get; set; }
    public IEnumerable<Route> Routes { get; set; } = [];
    public IDictionary<string, Service> Services { get; set; } = new Dictionary<string, Service>();
}