using Consul;
using Consul.Filtering;
using Jalpan.Discovery.Consul.Exceptions;
using Microsoft.Extensions.Options;

namespace Jalpan.Discovery.Consul;

internal sealed class ConsulHttpHandler(IConsulClient client, IOptions<ConsulOptions> options) : DelegatingHandler
{
    private const string HostDockerInternal = "host.docker.internal";
    private readonly StringFieldSelector _selector = new("Service");
    private readonly IConsulClient _client = client;
    private readonly bool _enabled = options.Value.Enabled;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_enabled || request.RequestUri is null || string.IsNullOrWhiteSpace(request.RequestUri.Host))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var serviceName = request.RequestUri.Host;

        var services = await _client.Agent.Services(_selector == serviceName, cancellationToken);
        if (services.Response.Count == 0)
        {
            throw new ServiceNotFoundException(serviceName);
        }

        var service = services.Response.First().Value;
        if (service.Address.Contains(HostDockerInternal))
        {
            service.Address = service.Address.Replace(HostDockerInternal, "localhost");
        }

        request.RequestUri = new UriBuilder(request.RequestUri)
        {
            Host = service.Address,
            Port = service.Port
        }.Uri;

        return await base.SendAsync(request, cancellationToken);
    }
}