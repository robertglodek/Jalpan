namespace Jalpan.Messaging.Idempotency.Outbox;

internal sealed class MessageInboxConfigurator(IJalpanBuilder builder, OutboxOptions options) : IMessageOutboxConfigurator
{
    public IJalpanBuilder Builder { get; } = builder;
    public OutboxOptions Options { get; } = options;
}
