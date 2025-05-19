using Taskly.Services.Notification.Domain.Enums;
using Taskly.Services.Notification.Domain.ValueObjects;

namespace Taskly.Services.Notification.Domain.Entities;

public sealed class RoleAssignment : AggregateRoot
{
    public Role Role { get; private set; }
    public Constraint Constraint { get; private set; }
    public string Value { get; private set; }

    public RoleAssignment(Role role, string value)
    {
        Role = role;
        Value = value;
    }
}
