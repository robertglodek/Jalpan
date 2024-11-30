using Microsoft.AspNetCore.Routing;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Jalpan.WebApi.MinimalApi;

public static class Extensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group, string prefix = "api")
    {
        var groupName = group.GetType().Name;

        return app.MapGroup($"/{prefix}/{groupName}".Replace("//", "/").ToLower())
                  .WithTags(groupName)
                  .WithOpenApi();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);
        var endpointGroupTypes = Assembly.GetCallingAssembly().GetTypes()
                                         .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }

    private static IEndpointRouteBuilder MapWithMethod(this IEndpointRouteBuilder builder,
        Delegate handler,
        Method method,
        string name = "",
        string pattern = "",
        Action<RouteHandlerBuilder>? config = null)
    {
        name = string.IsNullOrWhiteSpace(name) ? GetHandlerName(handler) : name;
        var routeBuilder = method switch
        {
            Method.Get => builder.MapGet(pattern, handler),
            Method.Post => builder.MapPost(pattern, handler),
            Method.Put => builder.MapPut(pattern, handler),
            Method.Delete => builder.MapDelete(pattern, handler),
            Method.Patch => builder.MapPatch(pattern, handler),
            _ => throw new ArgumentException("Invalid HTTP method.")
        };

        config?.Invoke(routeBuilder.WithName(name));
        return builder;
    }

    public static IEndpointRouteBuilder MapGet(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        string name = "",
        string pattern = "",
        Action<RouteHandlerBuilder>? config = null)
        => builder.MapWithMethod(handler, Method.Get, name, pattern, config);

    public static IEndpointRouteBuilder MapPost(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        string name = "",
        string pattern = "",
        Action<RouteHandlerBuilder>? config = null)
        => builder.MapWithMethod(handler, Method.Post, name, pattern, config);

    public static IEndpointRouteBuilder MapPut(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        string name = "",
        string pattern = "",
        Action<RouteHandlerBuilder>? config = null)
        => builder.MapWithMethod(handler, Method.Put, name, pattern, config);

    public static IEndpointRouteBuilder MapDelete(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        string name = "",
        string pattern = "",
        Action<RouteHandlerBuilder>? config = null)
        => builder.MapWithMethod(handler, Method.Delete, name, pattern, config);

    public static IEndpointRouteBuilder MapPatch(
        this IEndpointRouteBuilder builder, 
        Delegate handler, 
        string name = "", 
        string pattern = "", 
        Action<RouteHandlerBuilder>? config = null) 
        => builder.MapWithMethod(handler, Method.Patch, name, pattern, config);

    private static string GetHandlerName(Delegate handler)
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("When using anonymous handlers, the 'name' parameter must be provided.");
        }
        return handler.Method.Name;
    }

    private static bool IsAnonymous(this MethodInfo method)
        => method.Name.Any(c => c is '<' or '>');
    
    private enum Method : byte
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }
}
