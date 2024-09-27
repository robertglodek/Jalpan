using System.Collections.Concurrent;
using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Logging.Serilog.Decorators;

[Decorator]
internal sealed class LoggingCommandHandlerDecorator<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler, IContextProvider contextProvider,
    ILogger<LoggingCommandHandlerDecorator<TCommand, TResponse>> logger) : ICommandHandler<TCommand, TResponse> where TCommand : class, ICommand<TResponse>
{
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private readonly ICommandHandler<TCommand, TResponse> _handler = handler;
    private readonly IContextProvider _contextProvider = contextProvider;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand, TResponse>> _logger = logger;

    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var context = _contextProvider.Current();
        var name = Names.GetOrAdd(typeof(TCommand), command.GetType().Name.Underscore());
        _logger.LogInformation("Handling a command: {CommandName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}']...",
            name, context.ActivityId, context.MessageId, context.UserId);

        var result = await _handler.HandleAsync(command, cancellationToken);

        _logger.LogInformation("Handled a command: {CommandName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}]",
            name, context.ActivityId, context.MessageId, context.UserId);

        return result;
    }
}