namespace Taskly.Services.Note.Domain.Exceptions;

public sealed class InvalidNoteNameException() : DomainException("Invalid note name.")
{
    public override string Code => "invalid_note_name";
}