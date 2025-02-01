namespace Taskly.Services.Meta.Application.DTO;

public sealed class GoalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}