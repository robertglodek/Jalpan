namespace Taskly.Services.Meta.Domain.Exceptions;

public sealed class InvalidSectionNameException() : DomainException("Invalid section name.")
{
    public override string Code => "invalid_section_name";
}