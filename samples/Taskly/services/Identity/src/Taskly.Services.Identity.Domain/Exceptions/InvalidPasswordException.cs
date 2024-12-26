namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class InvalidPasswordException() : DomainException("Invalid password.")
{
    public override string Code => "invalid_password";
}