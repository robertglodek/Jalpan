using Jalpan.Messaging.Idempotency.Outbox;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Jalpan.Messaging.Idempotency.MongoDB.Outbox;

internal sealed class MongoDbOutboxInitializer : IInitializer
{
    private readonly IMongoDatabase _database;
    private readonly OutboxOptions _outboxOptions;
    private readonly MongoDbOutboxOptions _outboxMongoOptions;

    public MongoDbOutboxInitializer(IMongoDatabase database,
        IOptions<OutboxOptions> outboxOptions, IOptions<MongoDbOutboxOptions> outboxMongoOptions)
    {
        _database = database;
        _outboxOptions = outboxOptions.Value;
        _outboxMongoOptions = outboxMongoOptions.Value;
    }

    public async Task InitializeAsync()
    {
        if (!_outboxOptions.Enabled)
        {
            return;
        }

        if (_outboxMongoOptions.Expiry <= 0)
        {
            return;
        }

        var collection = string.IsNullOrWhiteSpace(_outboxMongoOptions.Collection)
           ? "outbox"
           : _outboxMongoOptions.Collection;

        var builder = Builders<OutboxMessage>.IndexKeys;
        await _database.GetCollection<OutboxMessage>(collection)
            .Indexes.CreateOneAsync(
                new CreateIndexModel<OutboxMessage>(builder.Ascending(i => i.SentAt),
                    new CreateIndexOptions
                    {
                        ExpireAfter = TimeSpan.FromSeconds(_outboxMongoOptions.Expiry)
                    }));
    }
}
