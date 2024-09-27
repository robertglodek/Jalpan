using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Messaging.Exceptions;
using Jalpan.Types;

namespace Jalpan.Messaging.RabbitMQ.Exceptions;

[Decorator]
internal sealed class MessagingErrorHandlingEventHandlerDecorator<T>(IEventHandler<T> handler, IContextProvider contextProvider,
    IMessagingExceptionPolicyHandler messagingExceptionPolicyHandler) : IEventHandler<T> where T : class, IEvent
{
    private readonly IEventHandler<T> _handler = handler;
    private readonly IContextProvider _contextProvider = contextProvider;
    private readonly IMessagingExceptionPolicyHandler _messagingExceptionPolicyHandler = messagingExceptionPolicyHandler;

    public Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var context = _contextProvider.Current();
        if (string.IsNullOrWhiteSpace(context.MessageId))
        {
            return _handler.HandleAsync(@event, cancellationToken);
        }

        return _messagingExceptionPolicyHandler.HandleAsync(@event, () => _handler.HandleAsync(@event, cancellationToken));
    }
}