namespace Jalpan.Messaging.RabbitMQ.Internals;

internal interface IMessageTypeRegistry
{
    void Register<T>();
    Type? Resolve(string type);
}