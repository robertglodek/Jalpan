namespace Taskly.Services.Meta.Application.DTO;

public sealed class TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Colour { get; init; } = null!;
    public Guid UserId { get; init; }
}