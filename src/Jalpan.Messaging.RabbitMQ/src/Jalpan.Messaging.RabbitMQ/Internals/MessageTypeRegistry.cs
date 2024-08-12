using System.Collections.Concurrent;

namespace Jalpan.Messaging.RabbitMQ.Internals;

internal sealed class MessageTypeRegistry : IMessageTypeRegistry
{
    private readonly ConcurrentDictionary<string, Type> _types = new();

    public void Register<T>() => _types[typeof(T).Name.ToMessageKey()] = typeof(T);

    public Type? Resolve(string type) => _types.TryGetValue(type, out var messageType) ? messageType : null;
}