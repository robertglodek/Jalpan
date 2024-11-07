namespace Jalpan.Messaging.Idempotency.Inbox;

public sealed class InboxOptions
{
    public bool Enabled { get; init; }
    public TimeSpan? CleanupInterval { get; init; }
}