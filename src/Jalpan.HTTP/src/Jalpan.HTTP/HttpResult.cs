namespace Jalpan.HTTP;

public sealed class HttpResult<T>(T result, HttpResponseMessage response)
{
    public T Result { get; } = result;
    public HttpResponseMessage Response { get; } = response;
    public bool HasResult => Result is not null;
}