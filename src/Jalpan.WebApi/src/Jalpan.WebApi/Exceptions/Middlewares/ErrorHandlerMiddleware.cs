using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Jalpan.WebApi.Exceptions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jalpan.WebApi.Exceptions.Middlewares;

internal sealed class ErrorHandlerMiddleware(
    IExceptionToResponseMapper exceptionToResponseMapper,
    ILogger<ErrorHandlerMiddleware> logger) : IMiddleware
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred while processing the request. Error: {Message}.", exception.Message);
            await HandleErrorAsync(context, exception);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var exceptionResponse = exceptionToResponseMapper.Map(exception);
        context.Response.StatusCode = (int)(exceptionResponse?.StatusCode ?? HttpStatusCode.InternalServerError);
        if (exceptionResponse?.Response is null)
        {
            await context.Response.WriteAsync(string.Empty);
            return;
        }

        await context.Response.WriteAsJsonAsync(exceptionResponse!.Response, _options);
    }
}
