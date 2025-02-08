namespace Taskly.Services.Meta.Application.Commands;

[UsedImplicitly]
public sealed class AddTag(string name, string? colour) : ICommand<Guid>
{
    public string Name { get; } = name;
    public string? Colour { get; } = colour;
}