namespace Taskly.Services.Notification.Application.Exceptions;

public sealed class UserAlreadyCreatedException(Guid userId) : AppException($"User with id: {userId} was already created.")
{
    public override string Code => "user_already_created";

    public Guid UserId { get; } = userId;
}
