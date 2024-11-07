namespace Jalpan.Messaging.RabbitMQ;

public sealed class RabbitMqOptions
{
    public bool Enabled { get; init; }
    public string ConnectionString { get; init; } = string.Empty;
}