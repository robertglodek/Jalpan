using Taskly.Services.Note.Application.Exceptions;
using Taskly.Services.Note.Domain.Repositories;
using Taskly.Services.Note.Domain.ValueObjects;

namespace Taskly.Services.Note.Application.Commands.Handlers;

[UsedImplicitly]
public sealed class UpdateNoteHandler(INoteRepository noteRepository, IContextProvider contextProvider)
    : ICommandHandler<UpdateNote, Empty>
{
    public async Task<Empty> HandleAsync(UpdateNote command, CancellationToken cancellationToken = default)
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
            
            note.UpdateName(command.Name);
            note.UpdateContent(command.Content);
            note.UpdateLinks(command.Links?.Select(n => new Link(n.Url, n.Name)));
            
            await noteRepository.UpdateAsync(note, cancellationToken);
        });
}