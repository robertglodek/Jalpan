using Jalpan.Types;

namespace Jalpan.Messaging.Exceptions;

internal class DefaultExceptionToMessageResolver : IExceptionToMessageResolver
{
    public IRejectedEvent? Map(ICommand command, Exception exception) => null;
}