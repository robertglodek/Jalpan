using System.Net.Http.Headers;
using Jalpan.HTTP.Serialization;

namespace Jalpan.HTTP.Client;

public interface IHttpClientAdapter
{
    Task<HttpResponseMessage> GetAsync(string uri);
    Task<T?> GetAsync<T>(string uri, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> GetResultAsync<T>(string uri, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> PostAsync(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> PostAsync(string uri, HttpContent content);
    Task<T?> PostAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<T?> PostAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> PostResultAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> PostResultAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> PutAsync(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> PutAsync(string uri, HttpContent content);
    Task<T?> PutAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<T?> PutAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> PutResultAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> PutResultAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> PatchAsync(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content);
    Task<T?> PatchAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<T?> PatchAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> PatchResultAsync<T>(string uri, object? data = null, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> PatchResultAsync<T>(string uri, HttpContent? content, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> DeleteAsync(string uri);
    Task<T?> DeleteAsync<T>(string uri, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> DeleteResultAsync<T>(string uri, IHttpClientSerializer? serializer = null);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    Task<T?> SendAsync<T>(HttpRequestMessage request, IHttpClientSerializer? serializer = null);
    Task<HttpResult<T?>> SendResultAsync<T>(HttpRequestMessage request, IHttpClientSerializer? serializer = null);
    IHttpClientAdapter SetHeaders(IDictionary<string, string> headers);
    IHttpClientAdapter SetHeaders(Action<HttpRequestHeaders> headers);
}