using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Messaging.Idempotency.Outbox;

[Decorator]
internal sealed class OutboxInstantSenderEventHandlerDecorator<T>(IEventHandler<T> handler, IOutbox outbox) : IEventHandler<T> where T : class, IEvent
{
    private readonly IEventHandler<T> _handler = handler;
    private readonly IOutbox _outbox = outbox;

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(@event, cancellationToken);
        await _outbox.PublishAwaitingAsync(cancellationToken);
    }
}
