namespace Taskly.Services.Note.Application.Exceptions;

public sealed class NoteNotFoundException(Guid tagId) : AppException($"Note with ID: '{tagId}' was not found.")
{
    public override string Code => "note_not_found";
}