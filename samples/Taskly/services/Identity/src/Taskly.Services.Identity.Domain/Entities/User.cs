using Taskly.Services.Identity.Domain.Exceptions;
using Taskly.Services.Identity.Domain.ValueObjects;

namespace Taskly.Services.Identity.Domain.Entities;

public sealed class User : AggregateRoot
{
    public string Email { get; private set; }
    public Role Role { get; private set; }
    public string Password { get; private set; }
    public DateTime? LastModifiedAt { get; set; }

    public User(Guid id, string email, string password, Role role, DateTime createdAt, DateTime? lastModifiedAt = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmailException(email);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidPasswordException();
        }

        Id = id;
        Email = email.ToLowerInvariant();
        Password = password;
        Role = role;
        CreatedAt = createdAt;
        LastModifiedAt = lastModifiedAt;
    }

    public void UpdatePassword(string password, DateTime modifiedAt)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidPasswordException();
        }

        Password = password;
        LastModifiedAt = modifiedAt;
    }

    public void UpdateEmail(string email, DateTime modifiedAt)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmailException(email);
        }

        Email = email;
        LastModifiedAt = modifiedAt;
    }
}