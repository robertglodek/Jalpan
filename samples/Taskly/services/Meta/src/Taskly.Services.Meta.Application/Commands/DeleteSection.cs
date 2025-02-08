namespace Taskly.Services.Meta.Application.Commands;

[UsedImplicitly]
public sealed class DeleteSection(Guid id) : ICommand<Empty>
{
    public Guid Id { get; } = id;
}