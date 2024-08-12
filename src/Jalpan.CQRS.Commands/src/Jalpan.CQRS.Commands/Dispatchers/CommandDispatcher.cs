using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.CQRS.Commands.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        => await (Task<TResult>)GetType().GetMethods().First(x => x.Name == "SendAsync" && x.GetGenericArguments().Length == 2)
                .MakeGenericMethod(command.GetType(), typeof(TResult)).Invoke(this, [command, cancellationToken]);

    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
       where TCommand : class, ICommand<TResult>
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, cancellationToken);
    }

}
