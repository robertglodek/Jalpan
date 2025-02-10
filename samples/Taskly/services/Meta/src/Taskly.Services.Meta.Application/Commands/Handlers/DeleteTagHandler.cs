using Taskly.Services.Meta.Application.Exceptions;
using Taskly.Services.Meta.Domain.Repositories;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class DeleteTagHandler(IContextProvider contextProvider, ITagRepository tagRepository)
    : ICommandHandler<DeleteTag, Empty>
{
    public async Task<Empty> HandleAsync(DeleteTag command, CancellationToken cancellationToken = default)
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

            await tagRepository.DeleteAsync(command.Id, cancellationToken);
        });
}