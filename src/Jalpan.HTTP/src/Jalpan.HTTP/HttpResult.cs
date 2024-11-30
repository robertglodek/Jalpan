namespace Jalpan.HTTP;

public class HttpResult<TResult>(TResult? result, HttpResponseMessage response)
{
    public TResult? Result { get; } = result;
    public HttpResponseMessage Response { get; } = response;
    public bool HasResult => Result is not null;
}

public sealed class HttpResult<TResult, TError>(TResult? result, TError? error, HttpResponseMessage response)
    : HttpResult<TResult>(result, response)
{
    public TError? Error { get; } = error;
    public bool HasError => Error is not null;
}