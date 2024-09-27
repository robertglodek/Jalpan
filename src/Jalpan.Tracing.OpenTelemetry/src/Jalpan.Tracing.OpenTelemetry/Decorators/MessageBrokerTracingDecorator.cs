using System.Collections.Concurrent;
using System.Diagnostics;
using Jalpan.Contexts.Providers;
using Jalpan.Messaging.Brokers;
using Jalpan.Types;

namespace Jalpan.Tracing.OpenTelemetry.Decorators;

internal sealed class MessageBrokerTracingDecorator(IMessageBroker messageBroker, IContextProvider contextProvider) : IMessageBroker
{
    public const string ActivitySourceName = "message_broker";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private readonly IMessageBroker _messageBroker = messageBroker;
    private readonly IContextProvider _contextProvider = contextProvider;

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        var context = _contextProvider.Current();
        var name = Names.GetOrAdd(typeof(T), message.GetType().Name.Underscore());
        using var activity = ActivitySource.StartActivity("publisher", ActivityKind.Producer, context.ActivityId);
        activity?.SetTag("message", name);
        activity?.SetTag("activity_id", context.ActivityId);
        if (!string.IsNullOrWhiteSpace(context.UserId))
        {
            activity?.SetTag("user_id", context.UserId);
        }
        
        await _messageBroker.SendAsync(message, cancellationToken);
    }
}