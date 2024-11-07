using Jalpan.Types;
using Jalpan.Handlers;

namespace Jalpan.Dispatchers;

internal sealed class InMemoryEventDispatcher(IServiceProvider serviceProvider) : IEventDispatcher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        if (@event is null)
        {
            throw new InvalidOperationException("Event cannot be null.");
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
        var tasks = handlers.Select(x => x.HandleAsync(@event, cancellationToken));
        await Task.WhenAll(tasks);
    }
}