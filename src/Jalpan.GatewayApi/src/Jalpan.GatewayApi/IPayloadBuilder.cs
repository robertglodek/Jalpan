using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi;

public interface IPayloadBuilder
{
    Task<string> BuildRawAsync(HttpRequest request);
    Task<T> BuildJsonAsync<T>(HttpRequest request) where T : class, new();
}