namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidRoleAssignmentValueException() : DomainException("Invalid role assignment value.")
{
    public override string Code => "invalid_role_assignment_value";
}
