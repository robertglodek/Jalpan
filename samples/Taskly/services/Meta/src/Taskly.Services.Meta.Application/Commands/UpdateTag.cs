namespace Taskly.Services.Meta.Application.Commands;

[UsedImplicitly]
public sealed class UpdateTag(Guid id, string name, string? colour) : ICommand<Empty>
{
    [Hidden]
    public Guid Id { get; set; } = id;
    public string Name { get; } = name;
    public string? Colour { get; } = colour;
}