using Jalpan.Types;
using Jalpan.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Dispatchers;

internal sealed class InMemoryEventDispatcher(IServiceProvider serviceProvider) : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        if (@event is null)
        {
            throw new InvalidOperationException("Event cannot be null.");
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
        var tasks = handlers.Select(x => x.HandleAsync(@event, cancellationToken));
        await Task.WhenAll(tasks);
    }
}