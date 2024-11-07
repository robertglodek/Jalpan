using System.Collections.Concurrent;
using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Logging.Serilog.Decorators;

[Decorator]
internal sealed class LoggingCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> handler,
    IContextProvider contextProvider,
    ILogger<LoggingCommandHandlerDecorator<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse> where TCommand : class, ICommand<TResponse>
{
    private static readonly ConcurrentDictionary<Type, string> Names = new();

    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var name = Names.GetOrAdd(typeof(TCommand), command.GetType().Name.Underscore());
        logger.LogInformation("Handling a command: {CommandName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}']...",
            name, context.ActivityId, context.MessageId, context.UserId);

        var result = await handler.HandleAsync(command, cancellationToken);

        logger.LogInformation("Handled a command: {CommandName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}]",
            name, context.ActivityId, context.MessageId, context.UserId);

        return result;
    }
}