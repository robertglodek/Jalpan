using Jalpan.Contexts;
using Jalpan.Messaging.Clients;
using Jalpan.Messaging.Idempotency.Outbox;
using Jalpan.Persistance.MongoDB.Repositories;
using Jalpan.Serialization;
using Jalpan.Time;
using Jalpan.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace Jalpan.Messaging.Idempotency.MongoDB.Outbox;

internal class MongoDbOutbox(IMongoDbRepository<OutboxMessage, string> outboxRepository, IDateTime dateTime,
    IMessageBrokerClient messageBrokerClient, IJsonSerializer jsonSerializer,
    IOptions<OutboxOptions> options, ILogger<MongoDbOutbox> logger) : IOutbox
{
    private static readonly ConcurrentDictionary<Type, Type> EnvelopeTypes = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> SendMethods = new();
    private static readonly ConcurrentDictionary<Type, string> MessageNames = new();
    private static readonly ConcurrentDictionary<string, Type> MessageTypes = new();
    private readonly List<OutboxMessage> _awaitingMessages = [];
    private readonly IMongoDbRepository<OutboxMessage, string> _outboxRepository = outboxRepository;
    private readonly IDateTime _dateTime = dateTime;
    private readonly IMessageBrokerClient _messageBrokerClient = messageBrokerClient;
    private readonly IJsonSerializer _jsonSerializer = jsonSerializer;
    private readonly ILogger<MongoDbOutbox> _logger = logger;
    private readonly MethodInfo _sendMethod = messageBrokerClient.GetType().GetMethod(nameof(IMessageBrokerClient.SendAsync)) ??
                      throw new InvalidOperationException("Send method was not defined.");

    public bool Enabled { get; } = options.Value.Enabled;

    public async Task SaveAsync<TMessage>(MessageEnvelope<TMessage> message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var outboxMessage = new OutboxMessage
        {
            Id = message.Context.MessageId,
            Context = _jsonSerializer.Serialize(message.Context),
            Name = message.Message.GetType().Name.Underscore(),
            Data = _jsonSerializer.Serialize<object>(message.Message),
            Type = message.Message.GetType().AssemblyQualifiedName ?? string.Empty,
            ReceivedAt = _dateTime.Now
        };

        await _outboxRepository.AddAsync(outboxMessage, cancellationToken: cancellationToken);
        _awaitingMessages.Add(outboxMessage);
        _logger.LogInformation($"Saved a message: '{outboxMessage.Name}' with ID: '{outboxMessage.Id}' to the outbox.");
    }

    public async Task PublishUnsentAsync(CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be sent.");
            return;
        }

        var unsentMessages = await _outboxRepository.FindAsync(x => x.SentAt == null, cancellationToken);
        if (!unsentMessages.Any())
        {
            _logger.LogInformation("No unsent messages found in outbox.");
            return;
        }

        _logger.LogInformation($"Found {unsentMessages.Count} unsent messages in outbox, sending...");
        foreach (var outboxMessage in unsentMessages.OrderBy(x => x.ReceivedAt))
        {
            await SendMessageAsync(outboxMessage, cancellationToken);
            outboxMessage.SentAt = _dateTime.Now;
            await _outboxRepository.UpdateAsync(outboxMessage, cancellationToken);
        }
    }

    public async Task PublishAwaitingAsync(CancellationToken cancellationToken = default)
    {
        if (_awaitingMessages.Count == 0)
        {
            return;
        }

        foreach (var outboxMessage in _awaitingMessages)
        {
            await SendMessageAsync(outboxMessage, cancellationToken);
            outboxMessage.SentAt = _dateTime.Now;
            await _outboxRepository.UpdateAsync(outboxMessage, cancellationToken);
            _awaitingMessages.Remove(outboxMessage);
        }
    }

    private async Task SendMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        var type = MessageTypes.GetOrAdd(outboxMessage.Type, _ =>
        {
            var type = Type.GetType(outboxMessage.Type);
            return type is null ? throw new InvalidOperationException($"Type was not found for: '{outboxMessage.Type}'.") : type;
        });

        if (_jsonSerializer.Deserialize(outboxMessage.Data, type) is not IMessage message)
        {
            throw new InvalidOperationException($"Invalid message type in outbox: '{type.Name}'," +
                                                $"name: '{outboxMessage.Name}', ID: '{outboxMessage.Id}'.");
        }

        var messageId = outboxMessage.Id;
        var context = _jsonSerializer.Deserialize<Context>(outboxMessage.Context) ?? new Context();
        var name = MessageNames.GetOrAdd(type, type.Name.Underscore());

        var envelopeType = EnvelopeTypes.GetOrAdd(type, typeof(MessageEnvelope<>).MakeGenericType(type));
        var sendMethod = SendMethods.GetOrAdd(type, _sendMethod.MakeGenericMethod(type));

        var messageContext = new MessageContext(messageId, context);
        var envelope = Activator.CreateInstance(envelopeType, message, messageContext);

        _logger.LogInformation("Sending a message from outbox: {Name} [Message ID: {MessageId}, Activity ID: {ActivityId}]...",
            name, messageId, context.ActivityId);

        var sendMessageTask = sendMethod.Invoke(_messageBrokerClient, [envelope, cancellationToken]) ?? throw new InvalidOperationException("Missing a task when sending a message.");
        await (Task)sendMessageTask;
    }

    public async Task CleanupAsync(DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? _dateTime.Now;
        var outboxMessages = await _outboxRepository.FindAsync(x => x.SentAt != null
                                                 && x.ReceivedAt <= dateTo, cancellationToken);
        if (!outboxMessages.Any())
        {
            _logger.LogInformation($"No sent messages found in outbox till: {dateTo}.");
            return;
        }

        _logger.LogInformation($"Found {outboxMessages.Count} sent messages in outbox till: {dateTo}, cleaning up...");
        await _outboxRepository.DeleteAsync(n => outboxMessages.Any(n => n.Id == n.Id), cancellationToken);
        _logger.LogInformation($"Removed {outboxMessages.Count} sent messages from outbox till: {dateTo}.");
    }
}
