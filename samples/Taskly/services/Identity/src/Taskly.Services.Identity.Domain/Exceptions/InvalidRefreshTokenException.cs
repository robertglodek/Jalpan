namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class InvalidRefreshTokenException : DomainException
{
    public override string Code { get; } = "invalid_refresh_token";
    
    public InvalidRefreshTokenException() : base("Invalid refresh token.")
    {
    }
}