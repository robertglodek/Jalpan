namespace Jalpan.Messaging.Idempotency.Outbox;

public sealed class OutboxOptions
{
    public bool Enabled { get; init; }
    public TimeSpan? SenderInterval { get; init; }
    public TimeSpan? CleanupInterval { get; init; }
}