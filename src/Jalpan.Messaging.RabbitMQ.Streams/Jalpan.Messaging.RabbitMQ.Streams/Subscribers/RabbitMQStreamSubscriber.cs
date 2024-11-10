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

internal sealed class RabbitMqStreamSubscriber(
    RabbitStreamManager streamManager,
    IStreamSerializer serializer,
    IOptions<RabbitMqStreamsOptions> options,
    ILogger<RabbitMqStreamSubscriber> logger)
    : IStreamSubscriber
{
    private readonly ConcurrentDictionary<string, Consumer> _consumers = new();
    private readonly bool _enabled = options.Value.Consumer?.Enabled ?? false;
    private readonly RabbitMqStreamsOptions.ConsumerOptions _consumerOptions = options.Value.Consumer ?? new RabbitMqStreamsOptions.ConsumerOptions();

    public async Task SubscribeAsync<T>(string stream, Func<T, Task> handler,
        CancellationToken cancellationToken = default) where T : class, IMessage
    {
        if (!_enabled)
        {
            logger.LogWarning("RabbitMQ Streams consumer is disabled, stream: '{Stream}' will not be subscribed.", stream);
            return;
        }

        if (_consumers.ContainsKey(stream))
        {
            return;
        }

        var lastOffset = await streamManager.GetLastOffset(stream);
        IOffsetType offsetType = _consumerOptions.OffsetType.ToLowerInvariant() switch
        {
            "first" => new OffsetTypeFirst(),
            "last" => new OffsetTypeLast(),
            "next" => new OffsetTypeNext(),
            "offset" => new OffsetTypeOffset(lastOffset),
            _ => throw new InvalidOperationException($"Unsupported offset type: '{_consumerOptions.OffsetType}'.")
        };

        var consumer = await streamManager.CreateConsumerAsync(stream, async (message, ctx) =>
        {
            var bytes = message.Data.Contents.ToArray();
            var payload = serializer.Deserialize<T>(bytes);
            if (payload is null)
            {
                logger.LogWarning("Received a null payload for message with offset: {Offset}.", ctx.Offset);
                return;
            }

            await handler(payload);
        }, offsetType, _consumerOptions.OffsetStorageThreshold);
        _consumers.TryAdd(stream, consumer);
    }
}