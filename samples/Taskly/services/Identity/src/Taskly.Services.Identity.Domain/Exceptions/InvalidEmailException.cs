namespace Taskly.Services.Identity.Core.Exceptions;

public sealed class InvalidEmailException(string email) : DomainException($"Invalid email: {email}.")
{
    public override string Code { get; } = "invalid_email";
}