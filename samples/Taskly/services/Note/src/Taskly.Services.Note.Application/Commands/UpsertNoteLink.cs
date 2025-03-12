namespace Taskly.Services.Note.Application.Commands;

[UsedImplicitly]
public sealed class UpsertNoteLink(string url, string? name = null)
{
    public string Url { get; set; } = url;
    public string? Name { get; set; } = name;
}