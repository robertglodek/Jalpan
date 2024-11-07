namespace Jalpan.Messaging.Idempotency.MongoDB.Outbox;

public sealed class MongoDbOutboxOptions
{
    public int Expiry { get; init; }

    public string Collection { get; init; } = null!;
}
