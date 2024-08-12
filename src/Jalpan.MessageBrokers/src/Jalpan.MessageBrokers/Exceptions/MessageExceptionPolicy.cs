using Jalpan.Messaging.Brokers;

namespace Jalpan.Messaging.Exceptions;

public record MessageExceptionPolicy(bool Retry, bool UseDeadLetter, Func<IMessageBroker, Task>? Publish = null);