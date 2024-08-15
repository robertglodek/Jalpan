namespace Jalpan.Messaging.Idempotency.Inbox;

public interface IMessageInboxConfigurator
{
    IJalpanBuilder Builder { get; }
    InboxOptions Options { get; }
}
