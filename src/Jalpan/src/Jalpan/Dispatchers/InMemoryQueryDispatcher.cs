using Jalpan.Types;
using Jalpan.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Dispatchers;

internal sealed class InMemoryQueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryQueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        if (query is null)
        {
            throw new InvalidOperationException("Query cannot be null.");
        }

#pragma warning disable CS8600
#pragma warning disable CS8602
        return await (Task<TResult>)GetType().GetMethods().First(x => x.Name == "QueryAsync" && x.GetGenericArguments().Length == 2)
               .MakeGenericMethod(query.GetType(), typeof(TResult)).Invoke(this, [query, cancellationToken]);
#pragma warning restore CS8602
#pragma warning restore CS8600
    }


    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : class, IQuery<TResult>
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(query, cancellationToken);
    }
}