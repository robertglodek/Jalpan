using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using EasyNetQ;
using Jalpan.Contexts;
using Jalpan.Contexts.Accessors;

namespace Jalpan.Messaging.RabbitMQ.Internals;

internal sealed class CustomMessageSerializationStrategy(IMessageTypeRegistry messageTypeRegistry, ISerializer serializer,
    IMessageContextAccessor messageContextAccessor, IContextAccessor contextAccessor) : IMessageSerializationStrategy
{
    private const string ActivityIdKey = "activity-id";
    private const string UserIdKey = "user-id";
    
    private readonly ConcurrentDictionary<Type, string> _typeNames = new();
    private readonly IMessageTypeRegistry _messageTypeRegistry = messageTypeRegistry;
    private readonly ISerializer _serializer = serializer;
    private readonly IMessageContextAccessor _messageContextAccessor = messageContextAccessor;
    private readonly IContextAccessor _contextAccessor = contextAccessor;

    public SerializedMessage SerializeMessage(IMessage message)
    {
        var messageContext = _messageContextAccessor.MessageContext;
        var messageBody = _serializer.MessageToBytes(message.MessageType, message.GetBody());
        var messageProperties = message.Properties;
        messageProperties.Type = _typeNames.GetOrAdd(message.MessageType, message.MessageType.Name.ToMessageKey());

        if (!string.IsNullOrWhiteSpace(messageContext?.MessageId))
        {
            messageProperties.MessageId = messageContext.MessageId;
        }

        if (!string.IsNullOrWhiteSpace(messageContext?.Context.UserId))
        {
            messageProperties.Headers.TryAdd(UserIdKey, messageContext.Context.UserId);
        }

        var isNewActivity = Activity.Current is null;
        var activity = Activity.Current ?? new Activity("message_serializer");
        if (!string.IsNullOrWhiteSpace(messageContext?.Context.ActivityId))
        {
            activity.SetParentId(messageContext.Context.ActivityId);
        }

        if (isNewActivity)
        {
            activity.Start();
        }
        
        messageProperties.Headers.TryAdd(ActivityIdKey, activity.Id);
        
        if (isNewActivity)
        {
            activity.Dispose();
        }

        return new SerializedMessage(messageProperties, messageBody);
    }

    public IMessage DeserializeMessage(MessageProperties properties, in ReadOnlyMemory<byte> body)
    {
        var type = _messageTypeRegistry.Resolve(properties.Type) ?? throw new Exception($"Message was not registered for type: '{properties.Type}'.");
        var activityId = GetHeaderValue(properties, ActivityIdKey);
        var userId = GetHeaderValue(properties, UserIdKey);

        _contextAccessor.Context = new Context(activityId, userId, properties.MessageId);

        var messageBody = _serializer.BytesToMessage(type, body);
        return MessageFactory.CreateInstance(type, messageBody, properties);
    }

    private static string GetHeaderValue(MessageProperties properties, string key)
        => properties.Headers.TryGetValue(key, out var bytes)
            ? Encoding.UTF8.GetString(bytes as byte[] ?? [])
            : string.Empty;
}