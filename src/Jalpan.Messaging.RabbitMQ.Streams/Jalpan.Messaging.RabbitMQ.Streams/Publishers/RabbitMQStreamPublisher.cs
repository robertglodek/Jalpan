using System.Collections.Concurrent;
using Jalpan.Messaging.Streams;
using Jalpan.Messaging.Streams.Serialization;
using Jalpan.Types;
using Microsoft.Extensions.Logging;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

namespace Jalpan.Messaging.RabbitMQ.Streams.Publishers;

internal sealed class RabbitMQStreamPublisher(RabbitStreamManager streamManager, IStreamSerializer serializer,
    ILogger<RabbitMQStreamPublisher> logger) : IStreamPublisher
{
    private readonly ConcurrentDictionary<Type, string> _names = new();
    private readonly RabbitStreamManager _streamManager = streamManager;
    private readonly IStreamSerializer _serializer = serializer;
    private readonly ConcurrentDictionary<string, Producer> _producers = new();
    private readonly ILogger<RabbitMQStreamPublisher> _logger = logger;

    public async Task PublishAsync<T>(string stream, T message, CancellationToken cancellationToken = default)
        where T : IMessage
    {
        if (!_producers.TryGetValue(stream, out var producer))
        {
            producer = await _streamManager.CreateProducerAsync(stream);
            _producers.TryAdd(stream, producer);
            _logger.LogInformation($"Created a new producer for stream: '{stream}'.");
        }

        var messageName = _names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());
        var payload = new Message(_serializer.Serialize(message));
        _logger.LogInformation("Sending a message: {MessageName} to the stream {Stream}...", messageName, stream);
        await _producers[stream].Send(payload);
    }
}