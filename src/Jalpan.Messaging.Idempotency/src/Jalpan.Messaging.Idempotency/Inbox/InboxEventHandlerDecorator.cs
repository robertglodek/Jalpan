using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Types;
using System.Collections.Concurrent;

namespace Jalpan.Messaging.Idempotency.Inbox;

[Decorator]
internal sealed class InboxEventHandlerDecorator<T>(IEventHandler<T> handler, IContextProvider contextProvider, IInbox inbox) : IEventHandler<T> where T : class, IEvent
{
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private readonly IEventHandler<T> _handler = handler;
    private readonly IContextProvider _contextProvider = contextProvider;
    private readonly IInbox _inbox = inbox;

    public Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var context = _contextProvider.Current();
        var messageName = Names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());

        return _inbox.Enabled && !string.IsNullOrWhiteSpace(context.MessageId) ? 
            _inbox.HandleAsync(context.MessageId, messageName, () => _handler.HandleAsync(@event, cancellationToken), cancellationToken)
            : 
            _handler.HandleAsync(@event, cancellationToken);
    }
}