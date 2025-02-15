using Taskly.Services.Meta.Application.DTO;

namespace Taskly.Services.Meta.Application.Queries;

public sealed class GetGoal : IQuery<GoalDto>
{
    public Guid Id { get; set; }
}