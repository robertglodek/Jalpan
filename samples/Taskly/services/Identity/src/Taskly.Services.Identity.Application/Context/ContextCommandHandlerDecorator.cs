using Jalpan.Contexts;
using Jalpan.Contexts.Accessors;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Domain.ValueObjects;

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

        await HAndler();

        return await handler.HandleAsync(command, cancellationToken);
    }


    public async Task HAndler()
    {
        await userRepository.GetAsync(Guid.NewGuid());
        var identityContext = new IdentityDataContext(null, "eo", Role.Admin);
        dataContextAccessor.DataContext = new DataContext<IdentityDataContext>(identityContext, null);
    }
}