using Jalpan.Types;

namespace Jalpan.Messaging.Exceptions;

public interface IMessagingExceptionPolicyHandler
{
    Task HandleAsync<T>(T message, Func<Task> handler) where T : IMessage;
}