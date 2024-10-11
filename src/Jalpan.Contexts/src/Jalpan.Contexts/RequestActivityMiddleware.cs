using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Jalpan.Contexts;

public sealed class RequestActivityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _parentActivityIdHeaderName;
    private readonly string _activityName;
    private readonly bool _overrideWhenExisting;

    public RequestActivityMiddleware(RequestDelegate next, string parentActivityIdHeaderName, string activityName, bool overrideWhenExisting)
    {
        _next = next;

        if (string.IsNullOrWhiteSpace(parentActivityIdHeaderName))
        {
            throw new ArgumentException("Parent activity ID header name cannot be null or empty.", nameof(parentActivityIdHeaderName));
        }

        if (string.IsNullOrWhiteSpace(activityName))
        {
            throw new ArgumentException("Activity name cannot be null or empty.", nameof(activityName));
        }

        _parentActivityIdHeaderName = parentActivityIdHeaderName;
        _activityName = activityName;
        _overrideWhenExisting = overrideWhenExisting;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        if (Activity.Current == null || _overrideWhenExisting)
        {
            using var activity = CreateActivity(context);

            activity.Start();

            AddRequestTags(activity, context);

            await _next(context);

            var responseTime = DateTime.UtcNow - startTime;

            AddResponseTags(activity, context, startTime);
        }
        else
        {
            await _next(context);
        }
    }

    private Activity CreateActivity(HttpContext context)
    {
        var activity = new Activity(_activityName);

        if (context.Request.Headers.TryGetValue(_parentActivityIdHeaderName, out var traceId) && !string.IsNullOrEmpty(traceId))
        {
            activity.SetParentId(traceId.ToString());
        }

        return activity;
    }

    private static void AddRequestTags(Activity activity, HttpContext context)
    {
        activity.AddTag("http.method", context.Request.Method);
        activity.AddTag("http.url", context.Request.Path);
        activity.AddTag("http.user_agent", context.Request.Headers.UserAgent.ToString());
        activity.AddTag("http.client_ip", context.Connection.RemoteIpAddress?.ToString());
        activity.AddTag("http.host", context.Request.Host.Value);
        activity.AddTag("http.query_string", context.Request.QueryString.ToString());
        activity.AddTag("app.name", "YourApplicationName");
        activity.AddTag("app.environment", "Production");
        activity.AddTag("correlation_id", context.TraceIdentifier);
    }

    private static void AddResponseTags(Activity activity, HttpContext context, DateTime startTime)
    {
        var responseTime = DateTime.UtcNow - startTime;

        activity.AddTag("http.status_code", context.Response.StatusCode.ToString());
        activity.AddTag("http.content_length", context.Response.ContentLength?.ToString() ?? "0");
        activity.AddTag("http.response_time", responseTime.TotalMilliseconds.ToString());
        activity.AddTag("http.protocol", context.Request.Protocol);
        activity.AddTag("http.is_successful", (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300).ToString());
    }
}
