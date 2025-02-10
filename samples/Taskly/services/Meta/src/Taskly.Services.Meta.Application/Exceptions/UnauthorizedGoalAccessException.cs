namespace Taskly.Services.Meta.Application.Exceptions;

public class UnauthorizedGoalAccessException(Guid goalId, Guid userId)
    : AppException($"Unauthorized access to goal: '{goalId}' by user: '{userId}'")
{
    public override string Code => "unauthorized_goal_access";
}