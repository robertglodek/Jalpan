using Taskly.Services.Identity.Application.Context;
using Taskly.Services.Identity.Application.Exceptions;

namespace Taskly.Services.Identity.Application.Lock;

[Decorator]
internal sealed class IsUserLockedQueryHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> handler,
    IDataContextProvider<IdentityDataContext> dataContextProvider,
    IDateTime dateTime)
    : IQueryHandler<TQuery, TResponse> where TQuery : class, IQuery<TResponse>
{
    public async Task<TResponse?> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var dataContext = dataContextProvider.Current();
        var userId = dataContext.Context.UserId;
        var lockTo = dataContext.Data?.LockTo;

        if (userId is not null && lockTo is not null && lockTo > dateTime.Now)
        {
            throw new UserLockedException(userId, lockTo.Value);
        }

        return await handler.HandleAsync(query, cancellationToken);
    }
}