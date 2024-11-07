using Jalpan.Types;
using Jalpan.Handlers;

namespace Jalpan.Dispatchers;

public class InMemoryDispatcher(
    ICommandDispatcher commandDispatcher,
    IEventDispatcher eventDispatcher,
    IQueryDispatcher queryDispatcher) : IDispatcher
{
    public async Task<TResult?> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        => await queryDispatcher.QueryAsync(query, cancellationToken);

    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
       => commandDispatcher.SendAsync(command, cancellationToken);

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
          => await eventDispatcher.PublishAsync(@event, cancellationToken);
}
