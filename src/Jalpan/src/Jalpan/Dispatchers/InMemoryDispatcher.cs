using Jalpan.Types;
using Jalpan.Handlers;

namespace Jalpan.Dispatchers;

public class InMemoryDispatcher(
    ICommandDispatcher commandDispatcher,
    IEventDispatcher eventDispatcher,
    IQueryDispatcher queryDispatcher) : IDispatcher
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IEventDispatcher _eventDispatcher = eventDispatcher;
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        => await _queryDispatcher.QueryAsync(query, cancellationToken);

    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
       => _commandDispatcher.SendAsync(command, cancellationToken);

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
          => await _eventDispatcher.PublishAsync(@event, cancellationToken);
}
