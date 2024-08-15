namespace Jalpan.Messaging.Idempotency.Outbox;

internal sealed class MessageInboxConfigurator : IMessageOutboxConfigurator
{
    public IJalpanBuilder Builder { get; }
    public OutboxOptions Options { get; }

    public MessageInboxConfigurator(IJalpanBuilder builder, OutboxOptions options)
    {
        Builder = builder;
        Options = options;
    }
}
