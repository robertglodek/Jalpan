namespace Taskly.Services.Identity.Core.Exceptions;

public sealed class InvalidCredentialsException(string email) : DomainException("Invalid credentials.")
{
    public override string Code { get; } = "invalid_credentials";
    public string Email { get; } = email;
}