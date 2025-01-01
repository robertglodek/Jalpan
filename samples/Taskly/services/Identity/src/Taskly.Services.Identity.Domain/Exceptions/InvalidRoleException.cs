namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class InvalidRoleException(string role) : DomainException($"Invalid role: '{role}'.")
{
    public override string Code => "invalid_role";
}