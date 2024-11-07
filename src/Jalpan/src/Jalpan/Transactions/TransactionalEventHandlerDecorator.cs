using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Transactions;

[Decorator]
internal sealed class TransactionalEventHandlerDecorator<T>(IEventHandler<T> handler, IUnitOfWork unitOfWork)
    : IEventHandler<T> where T : class, IEvent
{
    public Task HandleAsync(T @event, CancellationToken cancellationToken = default)
        => unitOfWork.ExecuteAsync(() => handler.HandleAsync(@event, cancellationToken), cancellationToken);
}
