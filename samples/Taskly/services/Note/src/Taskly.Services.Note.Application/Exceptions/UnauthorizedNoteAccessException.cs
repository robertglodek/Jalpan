namespace Taskly.Services.Note.Application.Exceptions;

public class UnauthorizedNoteAccessException(Guid noteId, Guid userId)
    : AppException($"Unauthorized access to note: '{noteId}' by user: '{userId}'")
{
    public override string Code => "unauthorized_note_access";
}