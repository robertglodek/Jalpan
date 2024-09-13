using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Jalpan.Contexts;

public class EnsureActivityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _parentActivityIdHeaderName;

    public EnsureActivityMiddleware(RequestDelegate next, string parentActivityIdHeaderName)
    {
        _next = next;
        _parentActivityIdHeaderName = parentActivityIdHeaderName;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (Activity.Current == null)
        {
            var activity = new Activity("Jalpan.Contexts.HttpRequestIn");

            if (context.Request.Headers.TryGetValue(_parentActivityIdHeaderName, out var traceId) && !string.IsNullOrEmpty(traceId))
            {
                activity.SetParentId(traceId.ToString());
            }

            activity.Start();
            Activity.Current = activity;

            try
            {
                await _next(context);
            }
            finally
            {
                activity.Stop();
            }
        }
        else
        {
            await _next(context);
        }
    }
}