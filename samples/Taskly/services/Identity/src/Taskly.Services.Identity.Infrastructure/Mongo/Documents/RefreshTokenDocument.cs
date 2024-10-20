using Jalpan.Types;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Documents;

internal sealed class RefreshTokenDocument : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}