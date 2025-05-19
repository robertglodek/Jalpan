using Taskly.Services.Notification.Domain.Enums;
using Taskly.Services.Notification.Domain.ValueObjects;

namespace Taskly.Services.Notification.Domain.Entities;

public sealed class PermissionAssignment : AggregateRoot
{
    public Permission Permission { get; private set; }
    public Constraint Constraint { get; private set; }
    public string Value { get; private set; }

    public PermissionAssignment(Permission permission, string value)
    {
        Permission = permission;
        Value = value;
    }
}