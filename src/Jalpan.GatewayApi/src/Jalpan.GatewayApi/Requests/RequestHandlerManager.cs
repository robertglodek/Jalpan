using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jalpan.GatewayApi.Requests;

internal sealed class RequestHandlerManager : IRequestHandlerManager
{
    private readonly ILogger<IRequestHandlerManager> _logger;

    private static readonly ConcurrentDictionary<string, IHandler> Handlers =
        new ConcurrentDictionary<string, IHandler>();

    public RequestHandlerManager(ILogger<RequestHandlerManager> logger)
    {
        _logger = logger;
    }

    public IHandler Get(string name) => Handlers.TryGetValue(name, out var handler) ? handler : null;

    public void AddHandler(string name, IHandler handler)
    {
        if (Handlers.TryAdd(name, handler))
        {
            _logger.LogInformation($"Added a request handler: '{name}'");
            return;
        }

        _logger.LogError($"Couldn't add a request handler: '{name}'");
    }

    public async Task HandleAsync(string handler, HttpContext context, RouteConfig routeConfig)
    {
        if (!Handlers.TryGetValue(handler, out var instance))
        {
            throw new Exception($"Handler: '{handler}' was not found.");
        }

        await instance.HandleAsync(context, routeConfig);
    }
}