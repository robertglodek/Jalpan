namespace Jalpan.LoadBalancing.Fabio;

public sealed class FabioOptions
{
    public bool Enabled { get; init; }
    public string Url { get; init; } = string.Empty;
}