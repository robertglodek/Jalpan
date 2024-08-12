using Jalpan.Types;

namespace Jalpan.Handlers;

public interface ICommandHandler<in TCommand, TResult> where TCommand : class, ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand query, CancellationToken cancellationToken = default);
}