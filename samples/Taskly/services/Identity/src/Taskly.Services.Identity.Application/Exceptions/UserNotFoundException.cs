namespace Taskly.Services.Identity.Application.Exceptions;

public sealed class UserNotFoundException(Guid userId) : AppException($"User with ID: '{userId}' was not found.")
{
    public override string Code => "user_not_found";
    public Guid UserId { get; } = userId;
}