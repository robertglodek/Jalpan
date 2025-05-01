namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidEmailException(string email) : DomainException($"Invalid email: {email}.")
{
    public override string Code => "invalid_email";
}
