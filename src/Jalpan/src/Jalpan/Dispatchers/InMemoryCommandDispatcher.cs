﻿using Jalpan.Types;
using Jalpan.Handlers;

namespace Jalpan.Dispatchers;

internal sealed class InMemoryCommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        if (command is null)
        {
            throw new InvalidOperationException("Command cannot be null.");
        }

#pragma warning disable CS8600
#pragma warning disable CS8602
        return await (Task<TResult>)GetType().GetMethods().First(x => x.Name == "SendAsync" && x.GetGenericArguments().Length == 2)
                   .MakeGenericMethod(command.GetType(), typeof(TResult)).Invoke(this, [command, cancellationToken]);
#pragma warning restore CS8602
#pragma warning restore CS8600
    }

    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TResult>
    {
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, cancellationToken);
    }
}