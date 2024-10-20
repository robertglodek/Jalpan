namespace Taskly.Services.Identity.Core.Exceptions;

public sealed class RevokedRefreshTokenException : DomainException
{
    public override string Code { get; } = "revoked_refresh_token";
    
    public RevokedRefreshTokenException() : base("Revoked refresh token.")
    {
    }
}