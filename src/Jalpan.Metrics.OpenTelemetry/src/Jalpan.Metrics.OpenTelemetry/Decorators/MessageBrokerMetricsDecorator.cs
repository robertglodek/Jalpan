﻿using Jalpan.Types;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Jalpan.Messaging.Brokers;

namespace Jalpan.Metrics.OpenTelemetry.Decorators;

[Meter(MetricsKey)]
[UsedImplicitly]
internal sealed class MessageBrokerMetricsDecorator(IMessageBroker messageBroker) : IMessageBroker
{
    private const string MetricsKey = "message_broker";
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private static readonly Meter Meter = new(MetricsKey);
    private static readonly Counter<long> PublishedMessagesCounter = Meter.CreateCounter<long>("published_messages");

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        var name = Names.GetOrAdd(typeof(T), message.GetType().Name.Underscore());
        var tags = new KeyValuePair<string, object?>[]
        {
            new("message", name)
        };

        await messageBroker.SendAsync(message, cancellationToken);
        PublishedMessagesCounter.Add(1, tags);
    }
}
