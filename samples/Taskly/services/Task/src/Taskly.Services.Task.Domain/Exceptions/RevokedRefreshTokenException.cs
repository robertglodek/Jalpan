namespace Taskly.Services.Task.Domain.Exceptions;

public sealed class RevokedRefreshTokenException() : DomainException("Revoked refresh token.")
{
    public override string Code => "revoked_refresh_token";
}