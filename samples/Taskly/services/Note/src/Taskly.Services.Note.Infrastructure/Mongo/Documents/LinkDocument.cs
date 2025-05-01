namespace Taskly.Services.Note.Infrastructure.Mongo.Documents;

internal sealed class LinkDocument
{
    public string Url { get; init; } = null!;
    public string? Name { get; init; }
}
