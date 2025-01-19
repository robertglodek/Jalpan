namespace Taskly.Services.Task.Domain.Exceptions;

public sealed class InvalidCredentialsException(string email) : DomainException("Invalid credentials.")
{
    public string Email { get; } = email;
    public override string Code => "invalid_credentials";
}