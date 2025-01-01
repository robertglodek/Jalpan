using Jalpan.WebApi.Exceptions.Mappers;
using Taskly.Services.Identity.Api.Exceptions.Middlewares;

namespace Taskly.Services.Identity.Api.Exceptions;

public static class Extensions
{
    public static IServiceCollection AddErrorHandler<T>(this IServiceCollection services)
        where T : class, IExceptionToResponseMapper
    {
        services.AddTransient<IExceptionToResponseMapper, T>();
        services.AddTransient<ErrorHandlerMiddleware>();

        return services;
    }

    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app)
        => app.UseMiddleware<ErrorHandlerMiddleware>();
}