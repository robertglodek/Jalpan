using Jalpan.Messaging.Idempotency.Inbox;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Jalpan.Messaging.Idempotency.MongoDB.Inbox;

internal sealed class MongoDbInboxInitializer(IMongoDatabase database,
    IOptions<InboxOptions> inboxOptions, IOptions<MongoDbInboxOptions> inboxMongoOptions) : IInitializer
{
    private readonly InboxOptions _inboxOptions = inboxOptions.Value;
    private readonly MongoDbInboxOptions _inboxMongoOptions = inboxMongoOptions.Value;

    public async Task InitializeAsync()
    {
        if (!_inboxOptions.Enabled)
        {
            return;
        }

        if (_inboxMongoOptions.Expiry <= 0)
        {
            return;
        }

        var collection = string.IsNullOrWhiteSpace(_inboxMongoOptions.Collection)
            ? Extensions.DefaultInboxCollectionName : _inboxMongoOptions.Collection;

        var builder = Builders<InboxMessage>.IndexKeys;
        await database.GetCollection<InboxMessage>(collection)
            .Indexes.CreateOneAsync(
                new CreateIndexModel<InboxMessage>(builder.Ascending(i => i.ProcessedAt),
                    new CreateIndexOptions
                    {
                        ExpireAfter = TimeSpan.FromSeconds(_inboxMongoOptions.Expiry)
                    }));
    }
}