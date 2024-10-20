namespace Taskly.Services.Identity.Core.Exceptions;

public sealed class InvalidPasswordException : DomainException
{
    public override string Code { get; } = "invalid_password";

    public InvalidPasswordException() : base($"Invalid password.")
    {
    }
}