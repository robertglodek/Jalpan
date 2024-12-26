namespace Taskly.Services.Identity.Application.Exceptions;

public sealed class UserUnauthorizedException() : AppException("User not authorized.")
{
    public override string Code => "user_not_authorized";
}
