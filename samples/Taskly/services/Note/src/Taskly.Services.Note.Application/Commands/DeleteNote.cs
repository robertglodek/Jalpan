namespace Taskly.Services.Note.Application.Commands;

[UsedImplicitly]
public sealed class DeleteNote(Guid id) : ICommand<Empty>
{
    public Guid Id { get; } = id;
}