using Jalpan.Contexts;
using Jalpan.Contexts.Accessors;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Context;

[Decorator]
internal sealed class ContextCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> handler,
    IContextProvider contextProvider,
    IDataContextAccessor<IdentityDataContext> dataContextAccessor,
    IUserRepository userRepository)
    : ICommandHandler<TCommand, TResponse> where TCommand : class, ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        if (context.UserId is null) return await handler.HandleAsync(command, cancellationToken);

        var user = await userRepository.GetAsync(Guid.Parse(context.UserId!));
        var identityContext = new IdentityDataContext(user!.Permissions, user.Email, user.Role);
        dataContextAccessor.DataContext = new DataContext<IdentityDataContext>(identityContext, context);

        return await handler.HandleAsync(command, cancellationToken);
    }
}