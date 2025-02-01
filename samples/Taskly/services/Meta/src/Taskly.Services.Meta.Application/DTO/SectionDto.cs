namespace Taskly.Services.Meta.Application.DTO;

public sealed class SectionDto
{
    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? GoalId { get; set; }
    public Guid UserId { get; set; }
}