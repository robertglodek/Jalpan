namespace Jalpan.LoadBalancing.Fabio;

public sealed class FabioOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; } = string.Empty;
}