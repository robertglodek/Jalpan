namespace Taskly.Services.Meta.Application.Exceptions;

public sealed class GoalNotFoundException(Guid goalId) : AppException($"Goal with ID: '{goalId}' was not found.")
{
    public override string Code => "goal_not_found";
}