namespace Jalpan.Messaging.Idempotency.MongoDB.Inbox;

public sealed class MongoDbInboxOptions
{
    public int Expiry { get; init; }
    public string Collection { get; init; } = null!;
}
