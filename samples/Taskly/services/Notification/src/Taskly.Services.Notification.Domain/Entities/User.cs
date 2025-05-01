using System.Runtime;
using Taskly.Services.Notification.Domain.Exceptions;
using Taskly.Services.Notification.Domain.ValueObjects;

namespace Taskly.Services.Notification.Domain.Entities;

public sealed class User : AggregateRoot
{
    public string Email { get; private set; }
    public Role Role { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public IEnumerable<string> Permissions { get; set; }
    public User(Guid id, string email, Role role, DateTime createdAt, DateTime? lastModifiedAt = null, IEnumerable<string>? permissions = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmailException(email);
        }

        Id = id;
        Email = email.ToLowerInvariant();
        Role = role;
        CreatedAt = createdAt;
        LastModifiedAt = lastModifiedAt;
        Permissions = permissions ?? [];
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmailException(email);
        }

        Email = email;
    }
}
