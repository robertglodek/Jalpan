using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Transactions;

[Decorator]
internal sealed class TransactionalQueryHandlerDecorator<TQuery, TResponse>(
    ICommandHandler<TQuery, TResponse> handler, IUnitOfWork unitOfWork) : ICommandHandler<TQuery, TResponse>
    where TQuery : class, ICommand<TResponse>
{
    public Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default) 
        => unitOfWork.ExecuteAsync(() => handler.HandleAsync(query, cancellationToken), cancellationToken);
}
