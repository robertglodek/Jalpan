using System.Net;
using Jalpan.WebApi.Exceptions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jalpan.WebApi.Exceptions.Middlewares;

internal sealed class ErrorHandlerMiddleware(
    IExceptionToResponseMapper exceptionToResponseMapper,
    ILogger<ErrorHandlerMiddleware> logger) : IMiddleware
{
    private readonly IExceptionToResponseMapper _exceptionToResponseMapper = exceptionToResponseMapper;
    private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleErrorAsync(context, exception);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var exceptionResponse = _exceptionToResponseMapper.Map(exception);
        context.Response.StatusCode = (int)(exceptionResponse?.StatusCode ?? HttpStatusCode.InternalServerError);
        if (exceptionResponse?.Response is null)
        {
            await context.Response.WriteAsync(string.Empty);
            return;
        }

        await context.Response.WriteAsJsonAsync(exceptionResponse!.Response);
    }
}
