namespace Taskly.Services.Identity.Application.Events;

[PublicContract]
[Message("identity", "user_lock_set")]
public sealed class LockSet(Guid userId, DateTime? lockTo) : IEvent
{
    public Guid UserId { get; } = userId;
    public DateTime? LockTo { get; } = lockTo;
}