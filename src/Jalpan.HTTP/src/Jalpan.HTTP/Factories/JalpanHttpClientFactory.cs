using Jalpan.HTTP.Client;
using Jalpan.HTTP.Serialization;

namespace Jalpan.HTTP.Factories;

public sealed class JalpanHttpClientFactory(IHttpClientFactory httpClientFactory,
    IHttpClientSerializer httpClientSerializer) : IJalpanHttpClientFactory
{
    public IJalpanHttpClient CreateClient(string name)
        => new JalpanHttpClient(httpClientFactory.CreateClient(name), httpClientSerializer);
}