using Jalpan.CQRS.Commands;
using Jalpan.CQRS.Events;
using Jalpan.CQRS.Queries;

namespace Jalpan.WebApi.CQRS.Dispatchers;

public interface IDispatcher
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent;
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
