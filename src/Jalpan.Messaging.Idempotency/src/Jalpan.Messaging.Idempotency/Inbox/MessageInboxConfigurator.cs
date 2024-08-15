namespace Jalpan.Messaging.Idempotency.Inbox;

internal sealed class MessageInboxConfigurator : IMessageInboxConfigurator
{
    public IJalpanBuilder Builder { get; }
    public InboxOptions Options { get; }

    public MessageInboxConfigurator(IJalpanBuilder builder, InboxOptions options)
    {
        Builder = builder;
        Options = options;
    }
}
