using Jalpan.Messaging.Idempotency.Inbox;
using Microsoft.Extensions.Logging;
using Jalpan.Persistance.MongoDB.Repositories;
using Jalpan.Time;
using Microsoft.Extensions.Options;

namespace Jalpan.Messaging.Idempotency.MongoDB.Inbox;

internal class MongoDbInbox(IMongoDbRepository<InboxMessage, string> inboxRepository, IOptions<InboxOptions> options,
    ILogger<MongoDbInbox> logger, IDateTime dateTime) : IInbox
{
    private readonly IMongoDbRepository<InboxMessage, string> _inboxRepository = inboxRepository;
    private readonly ILogger<MongoDbInbox> _logger = logger;
    private readonly IDateTime _dateTime = dateTime;

    public bool Enabled { get; } = options.Value.Enabled;

    public async Task HandleAsync(string messageId, string messageName, Func<Task> handler, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Received a message with ID: '{messageId}' to be processed.");
        if (await _inboxRepository.ExistsAsync(m => m.Id == messageId, cancellationToken))
        {
            _logger.LogWarning($"Message with ID: '{messageId}' was already processed.");
            return;
        }

        _logger.LogInformation($"Processing a message with ID: '{messageId}'...");

        var inboxMessage = new InboxMessage
        {
            Id = messageId,
            Name = messageName,
            ReceivedAt = _dateTime.Now
        };

        await handler();

        inboxMessage.ProcessedAt = _dateTime.Now;
        await _inboxRepository.AddAsync(inboxMessage, cancellationToken: cancellationToken);
        _logger.LogInformation($"Processed a message with ID: '{messageId}'.");
    }

    public async Task CleanupAsync(DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, incoming messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? _dateTime.Now;
        var inboxMessages = await _inboxRepository.FindAsync(x => x.ReceivedAt <= dateTo, cancellationToken);
        if (!inboxMessages.Any())
        {
            _logger.LogInformation($"No received messages found in inbox till: {dateTo}.");
            return;
        }

        _logger.LogInformation($"Found {inboxMessages.Count} received messages in inbox till: {dateTo}, cleaning up...");

        await _inboxRepository.DeleteAsync(n => inboxMessages.Any(x => x.Id == n.Id), cancellationToken);
        _logger.LogInformation($"Removed {inboxMessages.Count} received messages from inbox till: {dateTo}.");
    }
}
