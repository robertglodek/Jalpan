using Jalpan.Types;

namespace Taskly.Services.Note.Infrastructure.Mongo.Documents;

internal sealed class NoteDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public Guid UserId { get; init; }



    internal sealed class LinkDocument
    {
        public string Url { get; init; } = null!;
        public string? Name { get; init; }
    }
}