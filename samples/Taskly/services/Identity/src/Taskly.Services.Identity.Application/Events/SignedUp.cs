namespace Taskly.Services.Identity.Application.Events;

[PublicContract]
public sealed class SignedUp(Guid userId, string email, string role) : IEvent
{
    public Guid UserId { get; } = userId;
    public string Email { get; } = email;
    public string Role { get; } = role;
}