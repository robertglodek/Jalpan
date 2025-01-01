using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Messaging.Brokers;
using Jalpan.Messaging.Exceptions;
using Jalpan.Types;

namespace Jalpan.Messaging.RabbitMQ.Exceptions;

[Decorator]
internal sealed class ErrorToMessageCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> handler,
    IExceptionToMessageResolver exceptionToMessageResolver,
    IMessageBroker messageBroker)
    : ICommandHandler<TCommand, TResponse> where TCommand : class, ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            return await handler.HandleAsync(command, cancellationToken);
        }
        catch (Exception e)
        {
            var @event = exceptionToMessageResolver.Map(command, e);
            if (@event != null)
            {
                await messageBroker.SendAsync(@event, cancellationToken);
            }
            throw;
        }
    }
}