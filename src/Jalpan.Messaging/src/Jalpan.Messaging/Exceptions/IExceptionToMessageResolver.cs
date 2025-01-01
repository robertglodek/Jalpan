using Jalpan.Types;

namespace Jalpan.Messaging.Exceptions;

public interface IExceptionToMessageResolver
{
    IRejectedEvent? Map(ICommand command, Exception exception);
}