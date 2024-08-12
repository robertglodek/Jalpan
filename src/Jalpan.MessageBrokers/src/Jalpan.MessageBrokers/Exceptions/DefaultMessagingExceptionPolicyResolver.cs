using Jalpan.Types;

namespace Jalpan.Messaging.Exceptions;

internal sealed class DefaultMessagingExceptionPolicyResolver : IMessagingExceptionPolicyResolver
{
    public MessageExceptionPolicy? Resolve(IMessage message, Exception exception) => null;
}