using System.Text.Json;
using System.Text.Json.Serialization;
using Jalpan.WebApi.Exceptions.Mappers;

namespace Taskly.Services.Identity.Api.Exceptions.Middlewares;

internal sealed class ErrorHandlerMiddleware(
    IExceptionToResponseMapper exceptionToResponseMapper,
    ILogger<ErrorHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the request. Error: {Message}", ex.Message);
            await HandleErrorAsync(context, ex);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var exceptionResponse = exceptionToResponseMapper.Map(exception);
        context.Response.StatusCode = (int)exceptionResponse!.StatusCode;
        await context.Response.WriteAsJsonAsync(exceptionResponse.Response, _options);
    }
}