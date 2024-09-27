using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Transactions;

[Decorator]
internal sealed class TransactionalEventHandlerDecorator<T>(IEventHandler<T> handler, IUnitOfWork unitOfWork) : IEventHandler<T>
    where T : class, IEvent
{
    private readonly IEventHandler<T> _handler = handler;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task HandleAsync(T @event, CancellationToken cancellationToken = default)
        => _unitOfWork.ExecuteAsync(() => _handler.HandleAsync(@event, cancellationToken), cancellationToken);
}
