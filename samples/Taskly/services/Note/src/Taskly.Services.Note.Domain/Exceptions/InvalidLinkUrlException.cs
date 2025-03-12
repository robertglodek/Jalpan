namespace Taskly.Services.Note.Domain.Exceptions;

public sealed class InvalidLinkUrlException() : DomainException("Invalid link url.")
{
    public override string Code => "invalid_link_url";
}