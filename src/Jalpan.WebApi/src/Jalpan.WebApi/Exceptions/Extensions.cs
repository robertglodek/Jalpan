using Jalpan.WebApi.Exceptions.Mappers;
using Jalpan.WebApi.Exceptions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.WebApi.Exceptions;

public static class Extensions
{
    public static IJalpanBuilder AddErrorHandler<T>(this IJalpanBuilder builder)
       where T : class, IExceptionToResponseMapper
    {
        builder.Services.AddTransient<ErrorHandlerMiddleware>();
        builder.Services.AddSingleton<IExceptionToResponseMapper, T>();

        return builder;
    }

    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        => builder.UseMiddleware<ErrorHandlerMiddleware>();
}
