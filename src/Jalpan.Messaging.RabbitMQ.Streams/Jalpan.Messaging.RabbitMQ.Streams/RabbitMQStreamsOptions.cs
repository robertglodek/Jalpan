namespace Jalpan.Messaging.RabbitMQ.Streams;

public sealed class RabbitMqStreamsOptions
{
    public bool Enabled { get; init; }
    public string Server { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string VirtualHost { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public ConsumerOptions? Consumer { get; init; }
    public ProducerOptions? Producer { get; init; }

    public class ConsumerOptions
    {
        public bool Enabled { get; init; }
        public string Reference { get; init; } = string.Empty;
        public string OffsetType { get; init; } = "next";
        public ulong? OffsetStorageThreshold { get; init; }
        public IEnumerable<StreamOptions>? Streams { get; init; }
    }

    public class ProducerOptions
    {
        public string Reference { get; init; } = string.Empty;
        public IEnumerable<StreamOptions>? Streams { get; init; }
    }

    public sealed class StreamOptions
    {
        public string Name { get; init; } = string.Empty;
        public ulong? MaxLengthBytes { get; init; }
        public int? MaxSegmentSizeBytes { get; init; }
        public TimeSpan? MaxAge { get; init; }
    }
}