using Jalpan.Types;

namespace Jalpan.Messaging.Idempotency.Inbox;

public class InboxMessage : IIdentifiable<string>
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}