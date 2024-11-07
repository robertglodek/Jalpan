using Jalpan.Messaging.Idempotency.Inbox;
using Microsoft.Extensions.Logging;
using Jalpan.Persistence.MongoDB.Repositories;
using Jalpan.Time;
using Microsoft.Extensions.Options;

namespace Jalpan.Messaging.Idempotency.MongoDB.Inbox;

internal class MongoDbInbox(
    IMongoDbRepository<InboxMessage, string> inboxRepository,
    IOptions<InboxOptions> options,
    ILogger<MongoDbInbox> logger,
    IDateTime dateTime) : IInbox
{
    public bool Enabled { get; } = options.Value.Enabled;

    public async Task HandleAsync(string messageId, string messageName, Func<Task> handler, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Received a message with ID: '{MessageId}' to be processed.", messageId);
        if (await inboxRepository.ExistsAsync(m => m.Id == messageId, cancellationToken))
        {
            logger.LogWarning("Message with ID: '{MessageId}' was already processed.", messageId);
            return;
        }

        logger.LogInformation("Processing a message with ID: '{MessageId}'...", messageId);

        var inboxMessage = new InboxMessage
        {
            Id = messageId,
            Name = messageName,
            ReceivedAt = dateTime.Now
        };

        await handler();

        inboxMessage.ProcessedAt = dateTime.Now;
        await inboxRepository.AddAsync(inboxMessage, cancellationToken: cancellationToken);
        logger.LogInformation("Processed a message with ID: '{MessageId}'.", messageId);
    }

    public async Task CleanupAsync(DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            logger.LogWarning("Outbox is disabled, incoming messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? dateTime.Now;
        var inboxMessages = await inboxRepository.FindAsync(x => x.ReceivedAt <= dateTo, cancellationToken);
        if (!inboxMessages.Any())
        {
            logger.LogInformation("No received messages found in inbox till: {DateTo}.", dateTo);
            return;
        }

        logger.LogInformation("Found {Count} received messages in inbox till: {DateTo}, cleaning up...", inboxMessages.Count, dateTo);

        await inboxRepository.DeleteAsync(n => inboxMessages.Any(x => x.Id == n.Id), cancellationToken);
        
        logger.LogInformation("Removed {Count} received messages from inbox till: {DateTo}.", inboxMessages.Count, dateTo);
    }
}
