using Taskly.Services.Identity.Application.Context;
using Taskly.Services.Identity.Application.Exceptions;

namespace Taskly.Services.Identity.Application.Lock;

[Decorator]
internal sealed class IsUserLockedCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> handler,
    IDataContextProvider<IdentityDataContext> dataContextProvider,
    IDateTime dateTime)
    : ICommandHandler<TCommand, TResponse> where TCommand : class, ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var dataContext = dataContextProvider.Current();
        var userId = dataContext.Context.UserId;
        var lockTo = dataContext.Data?.LockTo;

        if (userId is not null && lockTo is not null && lockTo > dateTime.Now)
        {
            throw new UserLockedException(userId, lockTo.Value);
        }

        return await handler.HandleAsync(command, cancellationToken);
    }
}