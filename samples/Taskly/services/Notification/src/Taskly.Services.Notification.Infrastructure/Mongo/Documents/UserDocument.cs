using Jalpan.Types;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Documents;

internal sealed class UserDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public IEnumerable<string>? Permissions { get; set; }
}