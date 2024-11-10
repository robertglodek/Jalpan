using Taskly.Services.Identity.Domain.Exceptions;

namespace Taskly.Services.Identity.Domain.Entities;

public sealed class RefreshToken : AggregateRoot
{
    public AggregateId UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool Revoked => RevokedAt.HasValue;

    public RefreshToken(
        AggregateId id,
        AggregateId userId,
        string token,
        DateTime createdAt,
        DateTime? revokedAt = null)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new EmptyRefreshTokenException();
        }

        Id = id;
        UserId = userId;
        Token = token;
        CreatedAt = createdAt;
        RevokedAt = revokedAt;
    }

    public void Revoke(DateTime revokedAt)
    {
        if (Revoked)
        {
            throw new RevokedRefreshTokenException();
        }

        RevokedAt = revokedAt;
    }
}