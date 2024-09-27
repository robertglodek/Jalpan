namespace Jalpan.Messaging.Idempotency.MongoDB.Outbox;

public sealed class MongoDbOutboxOptions
{
    public int Expiry { get; set; }

    public string Collection { get; set; } = null!;
}
