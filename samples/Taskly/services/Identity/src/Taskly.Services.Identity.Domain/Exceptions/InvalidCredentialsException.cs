namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class InvalidCredentialsException() : DomainException("Invalid credentials.")
{
    public override string Code { get; } = "invalid_credentials";
}