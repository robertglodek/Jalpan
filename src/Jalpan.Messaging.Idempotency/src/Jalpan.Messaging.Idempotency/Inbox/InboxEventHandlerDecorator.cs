using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Types;
using System.Collections.Concurrent;

namespace Jalpan.Messaging.Idempotency.Inbox;

[Decorator]
internal sealed class InboxEventHandlerDecorator<T>(
    IEventHandler<T> handler, 
    IContextProvider contextProvider,
    IInbox inbox) : IEventHandler<T> where T : class, IEvent
{
    private static readonly ConcurrentDictionary<Type, string> Names = new();

    public Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var messageName = Names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());

        return inbox.Enabled && !string.IsNullOrWhiteSpace(context.MessageId) ? 
            inbox.HandleAsync(context.MessageId, messageName, () => handler.HandleAsync(@event, cancellationToken), cancellationToken)
            : 
            handler.HandleAsync(@event, cancellationToken);
    }
}