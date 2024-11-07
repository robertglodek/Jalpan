namespace Jalpan.WebApi.Networking;

public sealed class NetworkingOptions
{
    public bool Enabled { get; init; }
    public List<KnownNetwork> Networks { get; init; } = [];

    public sealed class KnownNetwork
    {
        public string Prefix { get; init; } = string.Empty;
        public int PrefixLength { get; init; }
    }
}