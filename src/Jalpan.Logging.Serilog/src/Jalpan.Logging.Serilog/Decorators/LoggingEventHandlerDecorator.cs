using System.Collections.Concurrent;
using Jalpan.Attributes;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Logging.Serilog.Decorators;

[Decorator]
internal sealed class LoggingEventHandlerDecorator<T>(
    IEventHandler<T> handler,
    IContextProvider contextProvider,
    ILogger<LoggingEventHandlerDecorator<T>> logger) : IEventHandler<T> where T : class, IEvent
{
    private static readonly ConcurrentDictionary<Type, string> Names = new();

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var name = Names.GetOrAdd(typeof(T), @event.GetType().Name.Underscore());
        logger.LogInformation("Handling an event: {EventName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}']...",
            name, context.ActivityId, context.MessageId, context.UserId);
        await handler.HandleAsync(@event, cancellationToken);
        logger.LogInformation("Handled an event: {EventName} [Activity ID: {ActivityId}, Message ID: {MessageId}, User ID: {UserId}]",
            name, context.ActivityId, context.MessageId, context.UserId);
    }
}