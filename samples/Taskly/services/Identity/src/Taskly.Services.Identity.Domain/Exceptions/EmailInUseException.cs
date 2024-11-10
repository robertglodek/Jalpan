namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class EmailInUseException(string email) : DomainException($"Email {email} is already in use.")
{
    public override string Code { get; } = "email_in_use";
    public string Email { get; } = email;
}