using Taskly.Services.Notification.Domain.Exceptions;

namespace Taskly.Services.Notification.Domain.ValueObjects;

public sealed class Role : ValueObject
{
    public string Value { get; private set; }

    private Role(string role)
    {
        Value = role;
    }

    public static Role From(string roleName)
    {
        var role = new Role(roleName);

        if (string.IsNullOrWhiteSpace(role) || !SupportedRoles.Contains(role))
        {
            throw new InvalidRoleException(roleName);
        }

        return role;
    }

    public static Role Standard => new("standard");
    public static Role Premium => new("premium");

    public static IEnumerable<Role> SupportedRoles
    {
        get
        {
            yield return Standard;
            yield return Premium;
        }
    }

    public static implicit operator string(Role role)
    {
        return role.ToString();
    }

    public static explicit operator Role(string roleName)
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
