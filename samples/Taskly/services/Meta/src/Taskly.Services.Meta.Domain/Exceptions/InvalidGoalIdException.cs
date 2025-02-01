namespace Taskly.Services.Meta.Domain.Exceptions;

public sealed class InvalidGoalIdException() : DomainException("Invalid goal identifier.")
{
    public override string Code => "invalid_goal_identifier";
}