using System.Collections.Concurrent;
using EasyNetQ;
using Jalpan.Contexts.Accessors;
using Jalpan.Messaging.Clients;
using Microsoft.Extensions.Logging;

namespace Jalpan.Messaging.RabbitMQ;

internal sealed class RabbitMqBrokerClient(IBus bus, IMessageContextAccessor messageContextAccessor,
    ILogger<RabbitMqBrokerClient> logger) : IMessageBrokerClient
{
    private readonly ConcurrentDictionary<Type, string> _names = new();
    private readonly IBus _bus = bus;
    private readonly IMessageContextAccessor _messageContextAccessor = messageContextAccessor;
    private readonly ILogger<RabbitMqBrokerClient> _logger = logger;

    public async Task SendAsync<T>(MessageEnvelope<T> messageEnvelope, CancellationToken cancellationToken = default)
        where T : Types.IMessage
    {
        var messageContext = messageEnvelope.Context;
        _messageContextAccessor.MessageContext = messageContext;
        var messageName = _names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());
        _logger.LogInformation("Sending a message: {MessageName}  [ID: {MessageId}, Activity ID: {ActivityId}]...",
            messageName, messageContext.MessageId, messageContext.Context.ActivityId);
        await _bus.PubSub.PublishAsync(messageEnvelope.Message, cancellationToken);
    }
}