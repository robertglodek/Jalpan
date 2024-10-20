using Jalpan.Types;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Documents;

internal sealed class UserDocument : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}