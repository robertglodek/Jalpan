namespace Taskly.Services.Meta.Domain.Exceptions;

public sealed class InvalidTagNameException() : DomainException("Invalid tag name.")
{
    public override string Code => "invalid_tag_name";
}