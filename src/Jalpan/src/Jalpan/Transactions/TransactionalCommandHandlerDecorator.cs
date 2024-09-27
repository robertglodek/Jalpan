using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Transactions;

[Decorator]
internal sealed class TransactionalCommandHandlerDecorator<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler, IUnitOfWork unitOfWork) 
    : ICommandHandler<TCommand, TResponse>
    where TCommand : class, ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _handler = handler;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default) 
        => _unitOfWork.ExecuteAsync(() => _handler.HandleAsync(command, cancellationToken), cancellationToken);
}
