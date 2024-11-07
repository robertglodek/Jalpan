using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Messaging.Idempotency.Outbox;

[Decorator]
internal sealed class OutboxInstantSenderEventHandlerDecorator<T>(
    IEventHandler<T> handler,
    IOutbox outbox) : IEventHandler<T> where T : class, IEvent
{
    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(@event, cancellationToken);
        await outbox.PublishAwaitingAsync(cancellationToken);
    }
}
