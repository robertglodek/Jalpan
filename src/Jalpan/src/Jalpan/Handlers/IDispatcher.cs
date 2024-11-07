using Jalpan.Types;

namespace Jalpan.Handlers;

public interface IDispatcher
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent;
    Task<TResult?> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}