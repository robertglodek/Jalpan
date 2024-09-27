using System.Net;

namespace Jalpan.WebApi.Exceptions;

public class ExceptionResponse(object response, HttpStatusCode statusCode)
{
    public object Response { get; } = response;
    public HttpStatusCode StatusCode { get; } = statusCode;
}
