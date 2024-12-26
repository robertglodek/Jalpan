using Jalpan.Types;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Documents;

internal sealed class RefreshTokenDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Token { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? RevokedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
}