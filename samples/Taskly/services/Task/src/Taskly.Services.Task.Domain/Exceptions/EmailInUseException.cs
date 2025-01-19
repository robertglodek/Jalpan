namespace Taskly.Services.Task.Domain.Exceptions;

public sealed class EmailInUseException(string email) : DomainException($"Email {email} is already in use.")
{
    public override string Code => "email_in_use";
    public string Email { get; } = email;
}