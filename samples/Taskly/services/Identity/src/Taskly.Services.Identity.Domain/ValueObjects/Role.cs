using Taskly.Services.Identity.Core.Exceptions;

namespace Taskly.Services.Identity.Domain.ValueObjects;

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

    public static Role User => new("user");
    public static Role Admin => new("admin");

    public static IEnumerable<Role> SupportedRoles
    {
        get
        {
            yield return User;
            yield return Admin;
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
