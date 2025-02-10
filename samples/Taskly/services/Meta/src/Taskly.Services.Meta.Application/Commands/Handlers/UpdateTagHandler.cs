using Taskly.Services.Meta.Application.Exceptions;
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Domain.ValueObjects;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class UpdateTagHandler(IContextProvider contextProvider, ITagRepository tagRepository)
    : ICommandHandler<UpdateTag, Empty>
{
    public async Task<Empty> HandleAsync(UpdateTag command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var tag = await tagRepository.GetAsync(command.Id, cancellationToken);
            if (tag == null)
            {
                throw new TagNotFoundException(command.Id);
            }

            if (tag.UserId != Guid.Parse(context.UserId!))
            {
                throw new UnauthorizedTagAccessException(tag.Id, Guid.Parse(context.UserId!));
            }
            
            var colour = string.IsNullOrWhiteSpace(command.Colour) ? Colour.Grey : command.Colour;
            
            tag.UpdateName(command.Name);
            tag.UpdateColour((Colour)colour);
            
            await tagRepository.UpdateAsync(tag, cancellationToken);
        });
}