using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Transactions;

[Decorator]
internal sealed class TransactionalCommandHandlerDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse> where TCommand : class, ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _handler;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionalCommandHandlerDecorator(ICommandHandler<TCommand, TResponse> handler, IUnitOfWork unitOfWork)
    {
        _handler = handler;
        _unitOfWork = unitOfWork;
    }

    public Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default) 
        => _unitOfWork.ExecuteAsync(() => _handler.HandleAsync(command, cancellationToken), cancellationToken);
}
