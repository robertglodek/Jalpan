namespace Taskly.Services.Meta.Application.Commands;

[UsedImplicitly]
public sealed class UpdateSection(Guid id, Guid? goalId, string name, string? description) : ICommand<Empty>
{
    [Hidden]
    public Guid Id { get; set; } = id;
    public Guid? GoalId { get; } = goalId;
    public string Name { get; } = name;
    public string? Description { get; } = description;
}