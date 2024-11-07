using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Messaging.Exceptions;
using Jalpan.Types;

namespace Jalpan.Messaging.RabbitMQ.Exceptions;

[Decorator]
internal sealed class MessagingErrorHandlingEventHandlerDecorator<T>(
    IEventHandler<T> handler,
    IContextProvider contextProvider,
    IMessagingExceptionPolicyHandler messagingExceptionPolicyHandler) : IEventHandler<T> where T : class, IEvent
{
    public Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        return string.IsNullOrWhiteSpace(context.MessageId) ? 
            handler.HandleAsync(@event, cancellationToken) : 
            messagingExceptionPolicyHandler.HandleAsync(@event, () => handler.HandleAsync(@event, cancellationToken));
    }
}