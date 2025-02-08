using Taskly.Services.Meta.Application.DTO;

namespace Taskly.Services.Meta.Application.Queries;

public sealed class GetSections : IQuery<IEnumerable<SectionDto>>
{
    public Guid? GoalId { get; set; }
}