namespace Taskly.Services.Meta.Application.Exceptions;

public class UnauthorizedTagAccessException(Guid tagId, Guid userId)
    : AppException($"Unauthorized access to tag: '{tagId}' by user: '{userId}'")
{
    public override string Code => "unauthorized_tag_access";
}