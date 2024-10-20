namespace Taskly.Services.Identity.Application.Events;

[PublicContract]
public sealed class SignedIn(Guid userId, string role) : IEvent
{
    public Guid UserId { get; } = userId;
    public string Role { get; } = role;
}