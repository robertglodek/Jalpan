using System.Collections.Concurrent;
using EasyNetQ;
using Jalpan.Contexts.Accessors;
using Jalpan.Messaging.Clients;
using Microsoft.Extensions.Logging;

namespace Jalpan.Messaging.RabbitMQ;

internal sealed class RabbitMqBrokerClient(IBus bus,
    IMessageContextAccessor messageContextAccessor,
    ILogger<RabbitMqBrokerClient> logger) : IMessageBrokerClient
{
    private readonly ConcurrentDictionary<Type, string> _names = new();

    public async Task SendAsync<T>(MessageEnvelope<T> messageEnvelope, CancellationToken cancellationToken = default)
        where T : Types.IMessage
    {
        var messageContext = messageEnvelope.Context;
        messageContextAccessor.MessageContext = messageContext;
        var messageName = _names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());
        logger.LogInformation("Sending a message: {MessageName}  [ID: {MessageId}, Activity ID: {ActivityId}]...",
            messageName, messageContext.MessageId, messageContext.Context.ActivityId);
        await bus.PubSub.PublishAsync(messageEnvelope.Message, cancellationToken);
    }
}