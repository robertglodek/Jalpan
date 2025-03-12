namespace Taskly.Services.Note.Application.Commands;

[UsedImplicitly]
public sealed class AddNote(string name, string content, IEnumerable<UpsertNoteLink>? links) : ICommand<Guid>
{
    public string Name { get; } = name;
    public string Content { get; } = content;
    public IEnumerable<UpsertNoteLink>? Links { get; } = links;
}