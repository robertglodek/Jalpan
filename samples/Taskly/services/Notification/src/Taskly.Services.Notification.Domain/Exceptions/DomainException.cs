namespace Taskly.Services.Notification.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message)
{
    public abstract string Code { get; }
}
