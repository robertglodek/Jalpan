using Jalpan.Types;

namespace Jalpan.Messaging.Exceptions;

internal class DefaultExceptionToMessageResolver : IExceptionToMessageResolver
{
    public IRejectedEvent? Resolve(ICommand command, Exception exception) => null;
}