namespace Jalpan.Messaging.Idempotency.Inbox;

public class InboxOptions
{
    public bool Enabled { get; set; }
    public TimeSpan? CleanupInterval { get; set; }
}