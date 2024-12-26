using Jalpan.Types;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Documents;

internal sealed class UserDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string Role { get; init; } = null!;
    public string? UiSettings { get; init; }
    public string Password { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public IEnumerable<string> Permissions { get; init; } = null!;
    public DateTime? LockTo { get; set; }
    public string? LockReason { get; set; }
}