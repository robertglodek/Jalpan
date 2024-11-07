using System.Buffers;
using System.Collections.Concurrent;
using Jalpan.Messaging.Streams;
using Jalpan.Messaging.Streams.Serialization;
using Jalpan.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

namespace Jalpan.Messaging.RabbitMQ.Streams.Subscribers;

internal sealed class RabbitMqStreamSubscriber : IStreamSubscriber
{
    private readonly ConcurrentDictionary<string, Consumer> _consumers = new();
    private readonly RabbitStreamManager _streamManager;
    private readonly IStreamSerializer _serializer;
    private readonly ILogger<RabbitMqStreamSubscriber> _logger;
    private readonly bool _enabled;
    private readonly RabbitMQStreamsOptions.ConsumerOptions _consumerOptions;

    public RabbitMqStreamSubscriber(RabbitStreamManager streamManager, IStreamSerializer serializer,
        IOptions<RabbitMQStreamsOptions> options, ILogger<RabbitMqStreamSubscriber> logger)
    {
        _streamManager = streamManager;
        _serializer = serializer;
        _logger = logger;
        _enabled = options.Value.Consumer?.Enabled ?? false;
        _consumerOptions = options.Value.Consumer ?? new RabbitMQStreamsOptions.ConsumerOptions();
        if (string.IsNullOrWhiteSpace(_consumerOptions.OffsetType))
        {
            _consumerOptions.OffsetType = "next";
        }
    }

    public async Task SubscribeAsync<T>(string stream, Func<T, Task> handler,
        CancellationToken cancellationToken = default) where T : class, IMessage
    {
        if (!_enabled)
        {
            _logger.LogWarning("RabbitMQ Streams consumer is disabled, stream: '{Stream}' will not be subscribed.", stream);
            return;
        }

        if (_consumers.ContainsKey(stream))
        {
            return;
        }

        var lastOffset = await _streamManager.GetLastOffset(stream);
        IOffsetType offsetType = _consumerOptions.OffsetType.ToLowerInvariant() switch
        {
            "first" => new OffsetTypeFirst(),
            "last" => new OffsetTypeLast(),
            "next" => new OffsetTypeNext(),
            "offset" => new OffsetTypeOffset(lastOffset),
            _ => throw new InvalidOperationException($"Unsupported offset type: '{_consumerOptions.OffsetType}'.")
        };

        var consumer = await _streamManager.CreateConsumerAsync(stream, async (message, ctx) =>
        {
            var bytes = message.Data.Contents.ToArray();
            var payload = _serializer.Deserialize<T>(bytes);
            if (payload is null)
            {
                _logger.LogWarning("Received a null payload for message with offset: {Offset}.", ctx.Offset);
                return;
            }

            await handler(payload);
        }, offsetType, _consumerOptions.OffsetStorageThreshold);
        _consumers.TryAdd(stream, consumer);
    }
}