using Taskly.Services.Meta.Application.Exceptions;
using Taskly.Services.Meta.Domain.Repositories;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class DeleteSectionHandler(IContextProvider contextProvider, ISectionRepository sectionRepository)
    : ICommandHandler<DeleteSection, Empty>
{
    public async Task<Empty> HandleAsync(DeleteSection command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var section = await sectionRepository.GetAsync(command.Id, cancellationToken);
            if (section == null)
            {
                throw new SectionNotFoundException(command.Id);
            }

            if (section.UserId != Guid.Parse(context.UserId!))
            {
                throw new UnauthorizedSectionAccessException(section.Id, Guid.Parse(context.UserId!));
            }

            await sectionRepository.DeleteAsync(command.Id, cancellationToken);
        });
}