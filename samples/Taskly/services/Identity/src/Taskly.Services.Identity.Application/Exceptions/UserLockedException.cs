namespace Taskly.Services.Identity.Application.Exceptions;

public sealed class UserLockedException(string userId, DateTime lockTo) 
    : AppException($"User with ID '{userId}' is locked out of the application until {lockTo:yyyy-MM-dd HH:mm:ss}.")
{
    public override string Code => "user_locked";
}