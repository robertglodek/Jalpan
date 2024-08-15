namespace Jalpan.Messaging.Idempotency.MongoDB.Inbox;

public class MongoDbInboxOptions
{
    public int Expiry { get; set; }

    public string Collection { get; set; } = null!;
}
