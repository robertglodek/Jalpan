using Jalpan.Types;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Documents;

internal sealed class TagDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Colour { get; init; } = null!;
    public Guid UserId { get; init; }
}