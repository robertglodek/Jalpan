using Jalpan.LoadBalancing.Fabio.Exceptions;
using Microsoft.Extensions.Options;

namespace Jalpan.LoadBalancing.Fabio;

internal sealed class FabioHttpHandler : DelegatingHandler
{
    private readonly bool _enabled;
    private readonly Uri _url;

    public FabioHttpHandler(IOptions<FabioOptions> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.Url))
        {
            throw new FabioConfigurationException("Fabio URL cannot be empty.");
        }

        _enabled = options.Value.Enabled;
        _url = new Uri(options.Value.Url);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!_enabled)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        if (request.RequestUri is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var serviceName = request.RequestUri.Host;
        var uriBuilder = new UriBuilder(request.RequestUri)
        {
            Host = _url.Host,
            Port = _url.Port,
            Path = $"{serviceName}{request.RequestUri.PathAndQuery}"
        };

        request.RequestUri = uriBuilder.Uri;

        return await base.SendAsync(request, cancellationToken);
    }
}