using Taskly.Services.Note.Domain.Repositories;
using Taskly.Services.Note.Domain.ValueObjects;

namespace Taskly.Services.Note.Application.Commands.Handlers;

[UsedImplicitly]
public sealed class AddNoteHandler(INoteRepository noteRepository, IContextProvider contextProvider)
    : ICommandHandler<AddNote, Guid>
{
    public async Task<Guid> HandleAsync(AddNote command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        
        var note = new Domain.Entities.Note(Guid.NewGuid(), Guid.Parse(context.UserId!), command.Name, command.Content,
            command.Links?.Select(n => new Link(n.Url, n.Name)));
        
        await noteRepository.AddAsync(note, cancellationToken);
        
        return note.Id;
    }
}