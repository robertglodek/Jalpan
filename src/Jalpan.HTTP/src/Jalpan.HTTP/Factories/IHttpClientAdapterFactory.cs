using Jalpan.HTTP.Client;

namespace Jalpan.HTTP.Factories;

public interface IHttpClientAdapterFactory
{
    IHttpClientAdapter CreateClient(string name);
}