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

    public static IApplicationBuilder UseRequestActivity(this IApplicationBuilder app,
        string parentActivityIdHeaderName = "traceparent",
        string activityName = "Service.HttpRequestIn",
        bool overrideWhenExisting = false)
        => app.UseMiddleware<RequestActivityMiddleware>(parentActivityIdHeaderName, activityName, overrideWhenExisting);
}