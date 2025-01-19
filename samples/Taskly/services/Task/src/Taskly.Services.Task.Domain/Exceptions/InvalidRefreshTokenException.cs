namespace Taskly.Services.Task.Domain.Exceptions;

public sealed class InvalidRefreshTokenException() : DomainException("Invalid refresh token.")
{
    public override string Code => "invalid_refresh_token";
}