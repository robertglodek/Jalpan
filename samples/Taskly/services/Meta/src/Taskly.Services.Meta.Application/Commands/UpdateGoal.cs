namespace Taskly.Services.Meta.Application.Commands;

[UsedImplicitly]
public sealed class UpdateGoal(Guid id, string name, string? description) : ICommand<Empty>
{
    [Hidden]
    public Guid Id { get; set; } = id;
    public string Name { get; } = name;
    public string? Description { get; } = description;
}