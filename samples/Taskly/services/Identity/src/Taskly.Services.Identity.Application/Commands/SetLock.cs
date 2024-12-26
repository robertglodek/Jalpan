namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class SetLock(Guid userId, DateTime? to, string? reason) : ICommand<Empty>
{
    public Guid UserId { get; set; } = userId;
    public DateTime? To { get; set; } = to;
    public string? Reason { get; set; } = reason;
}