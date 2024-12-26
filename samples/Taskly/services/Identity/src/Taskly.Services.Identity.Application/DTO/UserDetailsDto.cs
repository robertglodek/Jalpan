namespace Taskly.Services.Identity.Application.DTO;

public sealed class UserDetailsDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? UiSettings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public IEnumerable<string>? Permissions { get; set; }
    public DateTime? LockTo { get; set; }
    public string? LockReason { get; set; }
}