namespace Taskly.Services.Note.Domain.Exceptions;

public sealed class InvalidNoteContentException() : DomainException("Invalid note content.")
{
    public override string Code => "invalid_note_content";
}