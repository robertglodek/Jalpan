namespace Taskly.Services.Meta.Domain.Exceptions;

public sealed class InvalidGoalNameException() : DomainException("Invalid goal name.")
{
    public override string Code => "invalid_goal_name";
}