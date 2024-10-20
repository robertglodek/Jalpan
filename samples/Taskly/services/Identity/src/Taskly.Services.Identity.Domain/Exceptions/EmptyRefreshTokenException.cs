using Taskly.Services.Identity.Core.Exceptions;

namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class EmptyRefreshTokenException : DomainException
{
    public override string Code { get; } = "empty_refresh_token";

    public EmptyRefreshTokenException() : base("Empty refresh token.")
    {
    }
}