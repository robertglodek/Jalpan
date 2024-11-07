using System.Collections.Concurrent;
using System.Diagnostics;
using Jalpan.Contexts.Providers;
using Jalpan.Messaging.RabbitMQ.Internals;
using Jalpan.Types;

namespace Jalpan.Tracing.OpenTelemetry.Decorators;

internal sealed class MessageHandlerTracingDecorator(
    IMessageHandler messageHandler,
    IContextProvider contextProvider) : IMessageHandler
{
    public const string ActivitySourceName = "message_handler";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private static readonly ConcurrentDictionary<Type, string> Names = new();

    public async Task HandleAsync<T>(Func<IServiceProvider, T, CancellationToken, Task> handler, T message,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        var context = contextProvider.Current();
        var name = Names.GetOrAdd(typeof(T), message.GetType().Name.Underscore());
        using var activity = ActivitySource.StartActivity("subscriber", ActivityKind.Consumer, context.ActivityId);
        activity?.SetTag("message", name);
        activity?.SetTag("activity_id", context.ActivityId);
        if (!string.IsNullOrWhiteSpace(context.UserId))
        {
            activity?.SetTag("user_id", context.UserId);
        }

        try
        {
            await messageHandler.HandleAsync(handler, message, cancellationToken);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.ToString());
            throw;
        }
    }
}