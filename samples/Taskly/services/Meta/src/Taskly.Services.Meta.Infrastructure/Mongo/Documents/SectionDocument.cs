using Jalpan.Types;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Documents;

internal sealed class SectionDocument : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public Guid? GoalId { get; init; }
    public Guid UserId { get; init; }
}