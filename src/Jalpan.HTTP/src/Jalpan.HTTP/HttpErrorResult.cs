namespace Jalpan.HTTP;

public class HttpErrorResult<TError>(TError? error, HttpResponseMessage response)
{
    public TError? Error { get; } = error;
    public HttpResponseMessage Response { get; } = response;
    public bool HasError => Error is not null;
}