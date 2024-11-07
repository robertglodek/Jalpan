using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace Jalpan.WebApi.Networking;

public static class Extensions
{
    private const string DefaultSectionName = "networking";
    private const string XForwardedFor = "X-Forwarded-For";

    private static readonly IPNetwork LoopbackIPv4 = new(IPAddress.Parse("127.0.0.0"), 8);
    private static readonly IPNetwork LoopbackIPv6 = new(IPAddress.Parse("::1"), 128);
    private static readonly IPNetwork PrivateIPv410 = new(IPAddress.Parse("10.0.0.0"), 8);
    private static readonly IPNetwork PrivateIPv4172 = new(IPAddress.Parse("172.16.0.0"), 12);
    private static readonly IPNetwork PrivateIPv4192 = new(IPAddress.Parse("192.168.0.0"), 16);
    private static readonly IPNetwork Pv6Ula = new(IPAddress.Parse("fd00::"), 8);
    private static readonly IPNetwork Pv6MappedIPv4 = new(IPAddress.Parse("::ffff:10.0.0.0"), 8);

    public static IJalpanBuilder AddHeadersForwarding(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

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
            knownNetworks.Add(PrivateIPv410);
            knownNetworks.Add(PrivateIPv4172);
            knownNetworks.Add(PrivateIPv4192);
            knownNetworks.Add(Pv6Ula);
            knownNetworks.Add(Pv6MappedIPv4);
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