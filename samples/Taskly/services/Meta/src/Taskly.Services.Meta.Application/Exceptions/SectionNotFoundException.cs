namespace Taskly.Services.Meta.Application.Exceptions;

public sealed class SectionNotFoundException(Guid sectionId) : AppException($"Section with ID: '{sectionId}' was not found.")
{
    public override string Code => "section_not_found";
}