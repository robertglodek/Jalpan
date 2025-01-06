using Jalpan.Types;

namespace Jalpan.Messaging.Exceptions;

public interface IExceptionToMessageResolver
{
    IRejectedEvent? Resolve(ICommand command, Exception exception);
}