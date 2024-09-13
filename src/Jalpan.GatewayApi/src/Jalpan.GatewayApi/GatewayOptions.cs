using Jalpan.GatewayApi.Configuration;

namespace Jalpan.GatewayApi;

public class GatewayOptions
{
    public bool UseForwardedHeaders { get; set; }
    public bool? ForwardRequestHeaders { get; set; }
    public bool? ForwardResponseHeaders { get; set; }
    public IDictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>();
    public IEnumerable<string> ExcludeRequestHeaders { get; set; } = [];
    public IEnumerable<string> ExcludeResponseHeaders { get; set; } = [];
    public bool? PassQueryString { get; set; }
    public Configuration.Auth? Auth { get; set; }
    public bool? GenerateActivityId { get; set; }
    public IDictionary<string, Module> Modules { get; set; } = new Dictionary<string, Module>();
    public Http Http { get; set; } = null!;
    public LoadBalancer? LoadBalancer { get; set; }
    public bool UseLocalUrl { get; set; }
}