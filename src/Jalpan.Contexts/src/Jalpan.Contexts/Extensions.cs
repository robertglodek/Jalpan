using Jalpan.Contexts.Accessors;
using Jalpan.Contexts.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Contexts;

public static class Extensions
{
    public static IJalpanBuilder AddContexts(this IJalpanBuilder builder)
    {
        builder.Services.AddSingleton<IContextProvider, ContextProvider>();
        builder.Services.AddSingleton<IContextAccessor, ContextAccessor>();

        return builder;
    }

    public static IApplicationBuilder UseEnsureActivity(this IApplicationBuilder app,
       string headerName = "/traceparent")
       => app.UseMiddleware<EnsureActivityMiddleware>(string.IsNullOrWhiteSpace(headerName) ? "/traceparent": headerName);
}