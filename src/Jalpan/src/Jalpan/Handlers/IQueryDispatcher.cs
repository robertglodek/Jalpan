﻿using Jalpan.Types;

namespace Jalpan.Handlers;

public interface IQueryDispatcher
{
    Task<TResult?> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

    Task<TResult?> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>;
}