using Taskly.Services.Identity.Domain.ValueObjects;

namespace Taskly.Services.Identity.Application.Context;

public sealed class IdentityDataContext(IEnumerable<string> permissions, string email, Role role, DateTime? lockTo)
{
    public IEnumerable<string> Permissions { get; private set; } = permissions;
    public string Email { get; private set; } = email;
    public Role Role { get; private set; } = role;
    public DateTime? LockTo { get; private set; } = lockTo;
}