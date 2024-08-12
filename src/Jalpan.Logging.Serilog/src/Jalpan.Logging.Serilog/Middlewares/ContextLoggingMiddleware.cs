using Jalpan.Contexts;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Jalpan.Logging.Serilog.Middlewares;

internal sealed class ContextLoggingMiddleware : IMiddleware
{
    private readonly IContextProvider _contextProvider;

    public ContextLoggingMiddleware(IContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        var context = _contextProvider.Current();
        using (LogContext.PushProperty("ActivityId", context.ActivityId))
        {
            await next(httpContext);
        }
    }
}
