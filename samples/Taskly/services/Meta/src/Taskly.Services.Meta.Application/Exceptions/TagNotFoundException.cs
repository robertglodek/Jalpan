namespace Taskly.Services.Meta.Application.Exceptions;

public sealed class TagNotFoundException(Guid tagId) : AppException($"Tag with ID: '{tagId}' was not found.")
{
    public override string Code => "tag_not_found";
}