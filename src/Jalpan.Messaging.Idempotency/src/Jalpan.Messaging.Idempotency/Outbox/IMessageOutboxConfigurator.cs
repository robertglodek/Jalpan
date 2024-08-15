namespace Jalpan.Messaging.Idempotency.Outbox;

public interface IMessageOutboxConfigurator
{
    IJalpanBuilder Builder { get; }
    OutboxOptions Options { get; }
}
