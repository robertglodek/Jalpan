using System.Collections.Concurrent;
using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Logging.Serilog.Decorators;

[Decorator]
internal sealed class LoggingEventHandlerDecorator<T>(IEventHandler<T> handler, IContextProvider contextProvider,
    ILogger<LoggingEventHandlerDecorator<T>> logger) : IEventHandler<T> where T : class, IEvent
{
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private readonly IEventHandler<T> _handler = handler;
    private readonly IContextProvider _contextProvider = contextProvider;
    private readonly ILogger<LoggingEventHandlerDecorator<T>> _logger = logger;

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var context = _contextProvider.Current();
        var name = Names.GetOrAdd(typeof(T), @event.GetType().Name.Underscore());
        _logger.LogInformation("Handling an event: {EventName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}']...",
            name, context.ActivityId, context.MessageId, context.UserId);
        await _handler.HandleAsync(@event, cancellationToken);
        _logger.LogInformation("Handled an event: {EventName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}]",
            name, context.ActivityId, context.MessageId, context.UserId);
    }
}