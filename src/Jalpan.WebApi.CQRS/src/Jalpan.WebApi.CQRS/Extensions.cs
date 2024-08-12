using Jalpan.WebApi.CQRS.Dispatchers;
using Jalpan.WebApi.CQRS.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.WebApi.CQRS;

public static class Extensions
{
    public static IJalpanBuilder AddInMemoryDispatcher(this IJalpanBuilder builder)
    {
        builder.Services.AddSingleton<IDispatcher, InMemoryDispatcher>();
        return builder;
    }

    public static IApplicationBuilder UsePublicContracts<T>(
        this IApplicationBuilder app,
        string endpoint = "/contracts")
        => app.UsePublicContracts(endpoint, typeof(T));

    public static IApplicationBuilder UsePublicContracts(
        this IApplicationBuilder app,
        bool attributeRequired,
        string endpoint = "/contracts")
        => app.UsePublicContracts(endpoint, null, attributeRequired);

    public static IApplicationBuilder UsePublicContracts(
        this IApplicationBuilder app,
        string endpoint = "/contracts",
        Type? attributeType = null,
        bool attributeRequired = true)
        => app.UseMiddleware<PublicContractsMiddleware>(string.IsNullOrWhiteSpace(endpoint) ? "/contracts" :
            endpoint.StartsWith("/") ? endpoint : $"/{endpoint}", attributeType ?? typeof(PublicContractAttribute),
            attributeRequired);
}
