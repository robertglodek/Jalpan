using Jalpan.Contexts;
using Jalpan.Contexts.Accessors;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Context;

[Decorator]
internal sealed class ContextQueryHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> handler,
    IContextProvider contextProvider,
    IDataContextAccessor<IdentityDataContext> dataContextAccessor,
    IUserRepository userRepository)
    : IQueryHandler<TQuery, TResponse> where TQuery : class, IQuery<TResponse>
{
    public async Task<TResponse?> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        if (context.UserId is null) return await handler.HandleAsync(query, cancellationToken);

        var user = await userRepository.GetAsync(Guid.Parse(context.UserId!));
        var identityContext = new IdentityDataContext(user!.Permissions, user.Email, user.Role, user.LockTo);
        dataContextAccessor.DataContext = new DataContext<IdentityDataContext>(identityContext, context);

        return await handler.HandleAsync(query, cancellationToken);
    }
}