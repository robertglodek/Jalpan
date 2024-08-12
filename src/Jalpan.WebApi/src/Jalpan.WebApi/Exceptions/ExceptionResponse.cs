using System.Net;

namespace Jalpan.WebApi.Exceptions;

public class ExceptionResponse
{
    public object Response { get; }
    public HttpStatusCode StatusCode { get; }

    public ExceptionResponse(object response, HttpStatusCode statusCode)
    {
        Response = response;
        StatusCode = statusCode;
    }
}
