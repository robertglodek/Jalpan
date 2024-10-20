using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Jalpan.WebApi.MinimalApi;

public static class Extensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group, string prefix = "api")
    {
        var groupName = group.GetType().Name;

        return app
            .MapGroup($"/{prefix}/{groupName}".Replace("//", "/").ToLower())
            .WithTags(groupName)
            .WithOpenApi();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);

        var assembly = Assembly.GetCallingAssembly();

        var endpointGroupTypes = assembly.GetTypes()
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

    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, string name = "", [StringSyntax("Route")] string pattern = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AnonymousMethod(handler);
            name = handler.Method.Name;
        }

        builder.MapGet(pattern, handler).WithName(name);

        return builder;
    }

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, string name = "", [StringSyntax("Route")] string pattern = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AnonymousMethod(handler);
            name = handler.Method.Name;
        }

        builder.MapPost(pattern, handler).WithName(name);

        return builder;
    }

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, string name = "", [StringSyntax("Route")] string pattern = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AnonymousMethod(handler);
            name = handler.Method.Name;
        }

        builder.MapPut(pattern, handler).WithName(name);

        return builder;
    }

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, string name = "", [StringSyntax("Route")] string pattern = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AnonymousMethod(handler);
            name = handler.Method.Name;
        }

        builder.MapDelete(pattern, handler).WithName(name);

        return builder;
    }

    public static bool IsAnonymous(this MethodInfo method)
    {
        var invalidChars = new[] { '<', '>' };
        return method.Name.Any(invalidChars.Contains);
    }

    public static void AnonymousMethod(Delegate input)
    {
        if (input.Method.IsAnonymous())
        {
            throw new ArgumentException("When using anonymous handlers 'name' parameter must be provided.");
        }
    }
}
