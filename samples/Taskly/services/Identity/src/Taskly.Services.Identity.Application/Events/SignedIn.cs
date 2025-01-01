namespace Taskly.Services.Identity.Application.Events;

[PublicContract]
[Message("identity", "user_signed_in")]
public sealed class SignedIn(Guid userId, string role) : IEvent
{
    public Guid UserId { get; } = userId;
    public string Role { get; } = role;
}