using Jalpan.HTTP.Client;

namespace Jalpan.HTTP.Factories;

public interface IJalpanHttpClientFactory
{
    IJalpanHttpClient CreateClient(string name);
}