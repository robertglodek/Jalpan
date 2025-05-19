using Taskly.Services.Notification.Domain.Exceptions;

namespace Taskly.Services.Notification.Domain.ValueObjects;

public sealed class Permission : ValueObject
{
    public string Value { get; private set; }

    private Permission(string permission)
    {
        Value = permission;
    }

    public static Permission From(string permissionName)
    {
        var permission = new Permission(permissionName);

        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new InvalidPermissionException();
        }

        return permission;
    }

    public static Permission ExtendedNotificationSchedule => new("extended_notification_schedule");

    public static IEnumerable<Permission> SupportedPermissions
    {
        get
        {
            yield return ExtendedNotificationSchedule;
        }
    }

    public static implicit operator string(Permission role)
    {
        return role.ToString();
    }

    public static explicit operator Permission(string roleName)
    {
        return From(roleName);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

