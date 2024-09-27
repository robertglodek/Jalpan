using System.Reflection;
using EasyNetQ;
using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Messaging.RabbitMQ.Internals;
using Jalpan.Messaging.Subscribers;
using Jalpan.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.RabbitMQ;

internal sealed class RabbitMqMessageSubscriber(IMessageHandler messageHandler, IMessageTypeRegistry messageTypeRegistry, IBus bus) : IMessageSubscriber
{
    private readonly IMessageHandler _messageHandler = messageHandler;
    private readonly IMessageTypeRegistry _messageTypeRegistry = messageTypeRegistry;
    private readonly IBus _bus = bus;

    public IMessageSubscriber Event<T>() where T : class, IEvent
        => Message<T>((serviceProvider, @event, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().PublishAsync(@event, cancellationToken));

    public IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler)
        where T : class, Types.IMessage
    {
        _messageTypeRegistry.Register<T>();
        var messageAttribute = typeof(T).GetCustomAttribute<MessageAttribute>() ?? new MessageAttribute();

        _bus.PubSub.SubscribeAsync<T>(messageAttribute.SubscriptionId,
            (message, cancellationToken) => _messageHandler.HandleAsync(handler, message, cancellationToken),
            configuration =>
            {
                var topic = string.IsNullOrWhiteSpace(messageAttribute.Topic)
                    ? typeof(T).Name.ToMessageKey()
                    : messageAttribute.Topic;

                configuration.WithTopic(topic);
            });

        return this;
    }
}