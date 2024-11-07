using Jalpan.Contexts;
using Jalpan.Messaging.Clients;
using Jalpan.Messaging.Idempotency.Outbox;
using Jalpan.Persistence.MongoDB.Repositories;
using Jalpan.Serialization;
using Jalpan.Time;
using Jalpan.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace Jalpan.Messaging.Idempotency.MongoDB.Outbox;

internal class MongoDbOutbox(
    IMongoDbRepository<OutboxMessage, string> outboxRepository,
    IDateTime dateTime,
    IMessageBrokerClient messageBrokerClient,
    IJsonSerializer jsonSerializer,
    IOptions<OutboxOptions> options,
    ILogger<MongoDbOutbox> logger) : IOutbox
{
    private static readonly ConcurrentDictionary<Type, Type> EnvelopeTypes = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> SendMethods = new();
    private static readonly ConcurrentDictionary<Type, string> MessageNames = new();
    private static readonly ConcurrentDictionary<string, Type> MessageTypes = new();
    private readonly List<OutboxMessage> _awaitingMessages = [];
    private readonly MethodInfo _sendMethod = 
        messageBrokerClient.GetType().GetMethod(nameof(IMessageBrokerClient.SendAsync)) ??
                                              throw new InvalidOperationException("Send method was not defined.");
    public bool Enabled { get; } = options.Value.Enabled;

    public async Task SaveAsync<TMessage>(
        MessageEnvelope<TMessage> message,
        CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var outboxMessage = new OutboxMessage
        {
            Id = message.Context.MessageId,
            Context = jsonSerializer.Serialize(message.Context),
            Name = message.Message.GetType().Name.Underscore(),
            Data = jsonSerializer.Serialize<object>(message.Message),
            Type = message.Message.GetType().AssemblyQualifiedName ?? string.Empty,
            ReceivedAt = dateTime.Now
        };

        await outboxRepository.AddAsync(outboxMessage, cancellationToken: cancellationToken);
        _awaitingMessages.Add(outboxMessage);
        logger.LogInformation("Saved a message: '{OutboxMessageName}' with ID: '{OutboxMessageId}' to the outbox.", outboxMessage.Name, outboxMessage.Id);
    }

    public async Task PublishUnsentAsync(CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            logger.LogWarning("Outbox is disabled, outgoing messages won't be sent.");
            return;
        }

        var unsentMessages = await outboxRepository.FindAsync(x => x.SentAt == null, cancellationToken);
        if (!unsentMessages.Any())
        {
            logger.LogInformation("No unsent messages found in outbox.");
            return;
        }

        logger.LogInformation("Found {UnsentMessagesCount} unsent messages in outbox, sending...", unsentMessages.Count);
        foreach (var outboxMessage in unsentMessages.OrderBy(x => x.ReceivedAt))
        {
            await SendMessageAsync(outboxMessage, cancellationToken);
            outboxMessage.SentAt = dateTime.Now;
            await outboxRepository.UpdateAsync(outboxMessage, cancellationToken);
        }
    }

    public async Task PublishAwaitingAsync(CancellationToken cancellationToken = default)
    {
        if (_awaitingMessages.Count == 0)
        {
            return;
        }

        foreach (var outboxMessage in _awaitingMessages.ToList())
        {
            await SendMessageAsync(outboxMessage, cancellationToken);
            outboxMessage.SentAt = dateTime.Now;
            await outboxRepository.UpdateAsync(outboxMessage, cancellationToken);
            _awaitingMessages.Remove(outboxMessage);
        }
    }

    private async Task SendMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        var type = MessageTypes.GetOrAdd(outboxMessage.Type, _ =>
        {
            var type = Type.GetType(outboxMessage.Type);
            return type ?? throw new InvalidOperationException($"Type was not found for: '{outboxMessage.Type}'.");
        });

        if (jsonSerializer.Deserialize(outboxMessage.Data, type) is not IMessage message)
        {
            throw new InvalidOperationException($"Invalid message type in outbox: '{type.Name}'," +
                                                $"name: '{outboxMessage.Name}', ID: '{outboxMessage.Id}'.");
        }

        var messageId = outboxMessage.Id;
        var context = jsonSerializer.Deserialize<Context>(outboxMessage.Context) ?? new Context();
        var name = MessageNames.GetOrAdd(type, type.Name.Underscore());

        var envelopeType = EnvelopeTypes.GetOrAdd(type, typeof(MessageEnvelope<>).MakeGenericType(type));
        var sendMethod = SendMethods.GetOrAdd(type, _sendMethod.MakeGenericMethod(type));

        var messageContext = new MessageContext(messageId, context);
        var envelope = Activator.CreateInstance(envelopeType, message, messageContext);

        logger.LogInformation("Sending a message from outbox: {Name} [Message ID: {MessageId}, Activity ID: {ActivityId}]...",
            name, messageId, context.ActivityId);

        var sendMessageTask = sendMethod.Invoke(messageBrokerClient, [envelope, cancellationToken])
                              ?? throw new InvalidOperationException("Missing a task when sending a message.");
        await (Task)sendMessageTask;
    }

    public async Task CleanupAsync(DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            logger.LogWarning("Outbox is disabled, outgoing messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? dateTime.Now;
        var outboxMessages = await outboxRepository.FindAsync(x => x.SentAt != null
                                                 && x.ReceivedAt <= dateTo, cancellationToken);
        if (!outboxMessages.Any())
        {
            logger.LogInformation("No sent messages found in outbox till: {DateTo}.", dateTo);
            return;
        }

        logger.LogInformation("Found {OutboxMessagesCount} sent messages in outbox till: {DateTo}, cleaning up...", outboxMessages.Count, dateTo);
        await outboxRepository.DeleteAsync(n => outboxMessages.Any(s => s.Id == n.Id), cancellationToken);
        logger.LogInformation("Removed {OutboxMessagesCount} sent messages from outbox till: {dateTo}.", outboxMessages.Count, dateTo);
    }
}
