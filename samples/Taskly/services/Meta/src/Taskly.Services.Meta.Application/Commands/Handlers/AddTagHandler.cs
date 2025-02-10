using Taskly.Services.Meta.Domain.Entities;
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Domain.ValueObjects;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class AddTagHandler(IContextProvider contextProvider, ITagRepository tagRepository)
    : ICommandHandler<AddTag, Guid>
{
    public async Task<Guid> HandleAsync(AddTag command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        var colour = string.IsNullOrWhiteSpace(command.Colour) ? Colour.Grey : command.Colour;
        
        var tag = new Tag(Guid.NewGuid(), command.Name, Guid.Parse(context.UserId!), Colour.From(colour));
        await tagRepository.AddAsync(tag, cancellationToken);
        return tag.Id;
    }
}