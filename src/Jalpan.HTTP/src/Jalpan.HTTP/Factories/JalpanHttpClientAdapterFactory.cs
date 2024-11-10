using Jalpan.HTTP.Client;
using Jalpan.HTTP.Serialization;

namespace Jalpan.HTTP.Factories;

public sealed class JalpanHttpClientAdapterFactory(IHttpClientFactory httpClientFactory,
    IHttpClientSerializer httpClientSerializer) : IHttpClientAdapterFactory
{
    public IHttpClientAdapter CreateClient(string name)
        => new JalpanHttpClientAdapter(httpClientFactory.CreateClient(name), httpClientSerializer);
}