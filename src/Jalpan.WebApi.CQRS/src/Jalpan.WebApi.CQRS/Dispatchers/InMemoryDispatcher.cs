﻿using Jalpan.CQRS.Commands;
using Jalpan.CQRS.Events;
using Jalpan.CQRS.Queries;

namespace Jalpan.WebApi.CQRS.Dispatchers;

public class InMemoryDispatcher : IDispatcher
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public InMemoryDispatcher(
        ICommandDispatcher commandDispatcher,
        IEventDispatcher eventDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _eventDispatcher = eventDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        => await _queryDispatcher.QueryAsync(query, cancellationToken);

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        => await _commandDispatcher.SendAsync(command, cancellationToken);

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
          => await _eventDispatcher.PublishAsync(@event, cancellationToken);
}
