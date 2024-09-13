namespace Jalpan.GatewayApi.Configuration;

public class Route
{
    public string Upstream { get; set; } = null!;
    public string Method { get; set; } = null!;
    public string Downstream { get; set; } = null!;
    public string Use { get; set; } = null!;
    public string? ReturnValue { get; set; }
    public bool? PassQueryString { get; set; }
    public bool? Auth { get; set; }
    public bool? GenerateActivityId { get; set; }
    public IDictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>();
    public bool? ForwardRequestHeaders { get; set; }
    public bool? ForwardResponseHeaders { get; set; }
    public IEnumerable<string> ExcludeRequestHeaders { get; set; } = [];
    public IEnumerable<string> ExcludeResponseHeaders { get; set; } = [];
    public IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
    public IEnumerable<string> Policies { get; set; } = [];
}