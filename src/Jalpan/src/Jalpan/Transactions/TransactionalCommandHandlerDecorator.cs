using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Transactions;

[Decorator]
internal sealed class TransactionalCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> handler, IUnitOfWork unitOfWork) : ICommandHandler<TCommand, TResponse>
    where TCommand : class, ICommand<TResponse>
{
    public Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default) 
        => unitOfWork.ExecuteAsync(() => handler.HandleAsync(command, cancellationToken), cancellationToken);
}
