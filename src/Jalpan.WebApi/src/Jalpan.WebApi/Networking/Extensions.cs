using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Jalpan.WebApi.Networking;

public static class Extensions
{
    private const string SectionName = "networking";

    public static IServiceCollection AddHeadersForwarding(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        services.Configure<NetworkingOptions>(section);

        return services;
    }
        
    public static IApplicationBuilder UseHeadersForwarding(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<NetworkingOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }
        
        var knownNetworks = new List<Microsoft.AspNetCore.HttpOverrides.IPNetwork>();
        if (options.Networks.Count == 0)
        {
            knownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
            knownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("127.0.0.1"), 32));
        }
        else
        {
            options.Networks
                .ForEach(n => knownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse(n.Prefix), n.PrefixLength)));
        }

        var headersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All,
            ForwardLimit = null
        };

        headersOptions.KnownNetworks.Clear();
        knownNetworks.ForEach(headersOptions.KnownNetworks.Add);

        return app.UseForwardedHeaders(headersOptions);
    }
}