namespace Jalpan.Messaging.Idempotency.Inbox;

internal sealed class MessageInboxConfigurator(IJalpanBuilder builder, InboxOptions options) : IMessageInboxConfigurator
{
    public IJalpanBuilder Builder { get; } = builder;
    public InboxOptions Options { get; } = options;
}
