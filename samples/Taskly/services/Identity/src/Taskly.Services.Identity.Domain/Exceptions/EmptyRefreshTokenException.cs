namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class EmptyRefreshTokenException() : DomainException("Empty refresh token.")
{
    public override string Code => "empty_refresh_token";
}