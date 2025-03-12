using Taskly.Services.Note.Application.Exceptions;
using Taskly.Services.Note.Domain.Repositories;

namespace Taskly.Services.Note.Application.Commands.Handlers;

[UsedImplicitly]
public sealed class DeleteNoteHandler(INoteRepository noteRepository, IContextProvider contextProvider)
    : ICommandHandler<DeleteNote, Empty>
{
    public async Task<Empty> HandleAsync(DeleteNote command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var note = await noteRepository.GetAsync(command.Id, cancellationToken);
            if (note == null)
            {
                throw new NoteNotFoundException(command.Id);
            }

            if (note.UserId != Guid.Parse(context.UserId!))
            {
                throw new UnauthorizedNoteAccessException(note.Id, Guid.Parse(context.UserId!));
            }

            await noteRepository.DeleteAsync(command.Id, cancellationToken);
        });
    
}