using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace Jalpan.WebApi.Networking;

public static class Extensions
{
    private const string SectionName = "networking";
    private const string XForwardedFor = "X-Forwarded-For";

    private static readonly IPNetwork LoopbackIPv4 = new(IPAddress.Parse("127.0.0.0"), 8);
    private static readonly IPNetwork LoopbackIPv6 = new(IPAddress.Parse("::1"), 128);
    private static readonly IPNetwork PrivateIPv4_10 = new(IPAddress.Parse("10.0.0.0"), 8);
    private static readonly IPNetwork PrivateIPv4_172 = new(IPAddress.Parse("172.16.0.0"), 12);
    private static readonly IPNetwork PrivateIPv4_192 = new(IPAddress.Parse("192.168.0.0"), 16);
    private static readonly IPNetwork IPv6ULA = new(IPAddress.Parse("fd00::"), 8);
    private static readonly IPNetwork IPv6MappedIPv4 = new(IPAddress.Parse("::ffff:10.0.0.0"), 8);

    public static IJalpanBuilder AddHeadersForwarding(this IJalpanBuilder builder, string sectionName = SectionName)
    {
        if(string.IsNullOrEmpty(sectionName))
        {
            sectionName = SectionName;
        }

        var section = builder.Configuration.GetSection(sectionName);
        builder.Services.Configure<NetworkingOptions>(section);

        return builder;
    }
        
    public static IApplicationBuilder UseHeadersForwarding(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<NetworkingOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }

        var knownNetworks = new List<IPNetwork>();
        if (options.Networks.Count == 0)
        {
            knownNetworks.Add(LoopbackIPv4);
            knownNetworks.Add(LoopbackIPv6);
            knownNetworks.Add(PrivateIPv4_10);
            knownNetworks.Add(PrivateIPv4_172);
            knownNetworks.Add(PrivateIPv4_192);
            knownNetworks.Add(IPv6ULA);
            knownNetworks.Add(IPv6MappedIPv4);
        }
        else
        {
            options.Networks.ForEach(n => knownNetworks.Add(new IPNetwork(IPAddress.Parse(n.Prefix), n.PrefixLength)));
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

    public static string GetUserIpAddress(this HttpContext? context)
    {
        if (context is null)
        {
            return string.Empty;
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (!context.Request.Headers.TryGetValue(XForwardedFor, out var forwardedFor))
        {
            return string.Empty;
        }

        var ipAddresses = forwardedFor.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
        if (ipAddresses.Length != 0)
        {
            ipAddress = ipAddresses[0];
        }

        return ipAddress ?? string.Empty;
    }
}