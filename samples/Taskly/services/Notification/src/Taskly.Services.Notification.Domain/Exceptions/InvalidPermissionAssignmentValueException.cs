namespace Taskly.Services.Notification.Domain.Exceptions;

public sealed class InvalidPermissionAssignmentValueException() : DomainException("Invalid permission assignment value.")
{
    public override string Code => "invalid_permission_assignment_value";
}
