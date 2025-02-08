namespace Taskly.Services.Meta.Application.Commands;

[UsedImplicitly]
public sealed class AddGoal(string name, string? description) : ICommand<Guid>
{
    public string Name { get; } = name;
    public string? Description { get; } = description;
}