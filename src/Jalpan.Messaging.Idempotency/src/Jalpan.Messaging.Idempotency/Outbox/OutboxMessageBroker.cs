using System.Collections.Concurrent;
using Jalpan.Contexts;
using Jalpan.Contexts.Providers;
using Jalpan.Messaging.Brokers;
using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Messaging.Idempotency.Outbox;

internal sealed class OutboxMessageBroker(IOutbox outbox, IContextProvider contextProvider, ILogger<OutboxMessageBroker> logger) : IMessageBroker
{
    private readonly ConcurrentDictionary<Type, string> _names = new();
    private readonly IOutbox _outbox = outbox;
    private readonly IContextProvider _contextProvider = contextProvider;
    private readonly ILogger<OutboxMessageBroker> _logger = logger;

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        var messageId = Guid.NewGuid().ToString("N");
        var context = _contextProvider.Current();
        var messageName = _names.GetOrAdd(typeof(T), message.GetType().Name.Underscore());
        _logger.LogInformation("Saving a message to outbox: {MessageName}  [ID: {MessageId}, Activity ID: {ActivityId}]...",
            messageName, messageId, context.ActivityId);
        var messageEnvelope = new MessageEnvelope<T>(message, new MessageContext(messageId, context));
        await _outbox.SaveAsync(messageEnvelope, cancellationToken);
    }
}