namespace Taskly.Services.Meta.Application.Exceptions;

public class UnauthorizedSectionAccessException(Guid sectionId, Guid userId)
    : AppException($"Unauthorized access to section: '{sectionId}' by user: '{userId}'")
{
    public override string Code => "unauthorized_section_access";
}