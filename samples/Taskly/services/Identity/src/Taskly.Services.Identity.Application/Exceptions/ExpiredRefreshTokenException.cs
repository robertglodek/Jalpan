namespace Taskly.Services.Identity.Application.Exceptions;

public sealed class ExpiredRefreshTokenException() : AppException("Expired refresh token.")
{
    public override string Code => "expired_refresh_token";
}