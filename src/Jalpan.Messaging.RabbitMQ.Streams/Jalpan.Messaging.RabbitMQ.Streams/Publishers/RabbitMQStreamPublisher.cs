using System.Collections.Concurrent;
using Jalpan.Messaging.Streams;
using Jalpan.Messaging.Streams.Serialization;
using Jalpan.Types;
using Microsoft.Extensions.Logging;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

namespace Jalpan.Messaging.RabbitMQ.Streams.Publishers;

internal sealed class RabbitMqStreamPublisher(
    RabbitStreamManager streamManager,
    IStreamSerializer serializer,
    ILogger<RabbitMqStreamPublisher> logger) : IStreamPublisher
{
    private readonly ConcurrentDictionary<Type, string> _names = new();
    private readonly ConcurrentDictionary<string, Producer> _producers = new();

    public async Task PublishAsync<T>(string stream, T message, CancellationToken cancellationToken = default)
        where T : IMessage
    {
        if (!_producers.TryGetValue(stream, out var producer))
        {
            producer = await streamManager.CreateProducerAsync(stream);
            _producers.TryAdd(stream, producer);
            logger.LogInformation("Created a new producer for stream: '{Stream}'.", stream);
        }

        var messageName = _names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());
        var payload = new Message(serializer.Serialize(message));
        logger.LogInformation("Sending a message: {MessageName} to the stream {Stream}...", messageName, stream);
        await _producers[stream].Send(payload);
    }
}