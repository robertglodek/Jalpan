namespace Taskly.Services.Note.Domain.Exceptions;

public sealed class InvalidUserIdException() : DomainException("Invalid user identifier")
{
    public override string Code => "invalid_user_identifier";
}