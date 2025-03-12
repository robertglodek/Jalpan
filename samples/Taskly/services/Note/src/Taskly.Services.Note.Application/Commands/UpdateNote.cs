namespace Taskly.Services.Note.Application.Commands;

[UsedImplicitly]
public sealed class UpdateNote(Guid id, string name, string content, IEnumerable<UpsertNoteLink>? links)
    : ICommand<Empty>
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Content { get; } = content;
    public IEnumerable<UpsertNoteLink>? Links { get; } = links;
}