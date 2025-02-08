namespace Taskly.Services.Meta.Application.Commands;

[UsedImplicitly]
public sealed class DeleteTag(Guid id) : ICommand<Empty>
{
    public Guid Id { get; } = id;
}