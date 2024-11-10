using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Jalpan.HTTP.Serialization;

namespace Jalpan.HTTP.Client;

public sealed class JalpanHttpClientAdapter(HttpClient httpClient, IHttpClientSerializer httpClientSerializer)
    : IHttpClientAdapter
{
    public Task<HttpResponseMessage> GetAsync(string uri) => SendAsync(uri, Method.Get);

    public Task<T?> GetAsync<T>(string uri, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Get, serializer: serializer);

    public Task<HttpResult<T?>> GetResultAsync<T>(string uri, IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Get, serializer: serializer);

    public Task<HttpResponseMessage> PostAsync(string uri, object? data = null,
        IHttpClientSerializer? serializer = null)
        => SendAsync(uri, Method.Post, GetJsonPayload(data, serializer));

    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
        => SendAsync(uri, Method.Post, content);

    public Task<T?> PostAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Post, GetJsonPayload(data, serializer));

    public Task<T?> PostAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Post, content, serializer);

    public Task<HttpResult<T?>> PostResultAsync<T>(string uri, object? data = null,
        IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Post, GetJsonPayload(data, serializer), serializer);

    public Task<HttpResult<T?>> PostResultAsync<T>(string uri, HttpContent? content,
        IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Post, content, serializer);

    public Task<HttpResponseMessage> PutAsync(string uri, object? data = null,
        IHttpClientSerializer? serializer = null)
        => SendAsync(uri, Method.Put, GetJsonPayload(data, serializer));

    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
        => SendAsync(uri, Method.Put, content);

    public Task<T?> PutAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Put, GetJsonPayload(data, serializer), serializer);

    public Task<T?> PutAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Put, content, serializer);

    public Task<HttpResult<T?>> PutResultAsync<T>(string uri, object? data = null,
        IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Put, GetJsonPayload(data, serializer), serializer);

    public Task<HttpResult<T?>> PutResultAsync<T>(string uri, HttpContent? content,
        IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Put, content, serializer);

    public Task<HttpResponseMessage> PatchAsync(string uri, object? data = null,
        IHttpClientSerializer? serializer = null)
        => SendAsync(uri, Method.Patch, GetJsonPayload(data, serializer));

    public Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content)
        => SendAsync(uri, Method.Patch, content);

    public Task<T?> PatchAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Patch, GetJsonPayload(data, serializer));

    public Task<T?> PatchAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Patch, content, serializer);

    public Task<HttpResult<T?>> PatchResultAsync<T>(string uri, object? data = null,
        IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Patch, GetJsonPayload(data, serializer));

    public Task<HttpResult<T?>> PatchResultAsync<T>(string uri, HttpContent? content,
        IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Patch, content, serializer);

    public Task<HttpResponseMessage> DeleteAsync(string uri)
        => SendAsync(uri, Method.Delete);

    public Task<T?> DeleteAsync<T>(string uri, IHttpClientSerializer? serializer = null)
        => SendAsync<T>(uri, Method.Delete, serializer: serializer);

    public Task<HttpResult<T?>> DeleteResultAsync<T>(string uri, IHttpClientSerializer? serializer = null)
        => SendResultAsync<T>(uri, Method.Delete, serializer: serializer);

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        => await httpClient.SendAsync(request);

    public async Task<T?> SendAsync<T>(HttpRequestMessage request, IHttpClientSerializer? serializer = null)
    {
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        var stream = await response.Content.ReadAsStreamAsync();

        return await DeserializeJsonFromStream<T>(stream, serializer);
    }

    public async Task<HttpResult<T?>> SendResultAsync<T>(
        HttpRequestMessage request, 
        IHttpClientSerializer? serializer = null)
    {
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return new HttpResult<T?>(default, response);
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await DeserializeJsonFromStream<T>(stream, serializer);

        return new HttpResult<T?>(result, response);
    }

    public IHttpClientAdapter SetHeaders(IDictionary<string, string>? headers)
    {
        if (headers is null)
        {
            return this;
        }

        foreach (var (key, value) in headers)
        {
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }

        return this;
    }

    public IHttpClientAdapter SetHeaders(Action<HttpRequestHeaders>? headers)
    {
        headers?.Invoke(httpClient.DefaultRequestHeaders);
        return this;
    }

    private async Task<T?> SendAsync<T>(
        string uri,
        Method method,
        HttpContent? content = null,
        IHttpClientSerializer? serializer = null)
    {
        var response = await SendAsync(uri, method, content);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        var stream = await response.Content.ReadAsStreamAsync();

        return await DeserializeJsonFromStream<T>(stream, serializer);
    }

    private async Task<HttpResult<T?>> SendResultAsync<T>(
        string uri,
        Method method,
        HttpContent? content = null,
        IHttpClientSerializer? serializer = null)
    {
        var response = await SendAsync(uri, method, content);
        if (!response.IsSuccessStatusCode)
        {
            return new HttpResult<T?>(default, response);
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await DeserializeJsonFromStream<T>(stream, serializer);

        return new HttpResult<T?>(result, response);
    }

    private Task<HttpResponseMessage> SendAsync(
        string uri,
        Method method,
        HttpContent? content = null)
    {
        var requestUri = uri.StartsWith("http") ? uri : $"http://{uri}";

        return GetResponseAsync(requestUri, method, content);
    }

    private Task<HttpResponseMessage> GetResponseAsync(
        string uri,
        Method method,
        HttpContent? content = null)
        => method switch
        {
            Method.Get => httpClient.GetAsync(uri),
            Method.Post => httpClient.PostAsync(uri, content),
            Method.Put => httpClient.PutAsync(uri, content),
            Method.Patch => httpClient.PatchAsync(uri, content),
            Method.Delete => httpClient.DeleteAsync(uri),
            _ => throw new InvalidOperationException($"Unsupported HTTP method: {method}")
        };

    private StringContent? GetJsonPayload(object? data, IHttpClientSerializer? serializer = null)
    {
        if (data is null)
        {
            return null;
        }

        serializer ??= httpClientSerializer;
        var content = new StringContent(serializer.Serialize(data), Encoding.UTF8, MediaTypeNames.Application.Json);

        return content;
    }

    private async Task<T?> DeserializeJsonFromStream<T>(Stream stream, IHttpClientSerializer? serializer = null)
    {
        if (stream.CanRead is false)
        {
            return default;
        }

        serializer ??= httpClientSerializer;
        return await serializer.DeserializeAsync<T>(stream);
    }

    private enum Method
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }
}