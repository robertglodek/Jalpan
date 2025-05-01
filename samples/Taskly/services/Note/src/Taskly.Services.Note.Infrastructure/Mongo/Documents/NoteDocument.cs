using Jalpan.Types;

namespace Taskly.Services.Note.Infrastructure.Mongo.Documents;

internal sealed class NoteDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public Guid UserId { get; init; }
    public IEnumerable<LinkDocument>? Links { get; init; }
}