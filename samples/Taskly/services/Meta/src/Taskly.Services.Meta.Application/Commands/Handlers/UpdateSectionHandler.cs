using Taskly.Services.Meta.Application.Exceptions;
using Taskly.Services.Meta.Domain.Repositories;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class UpdateSectionHandler(IContextProvider contextProvider, ISectionRepository sectionRepository)
    : ICommandHandler<UpdateSection, Empty>
{
    public async Task<Empty> HandleAsync(UpdateSection command, CancellationToken cancellationToken = default)
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
            
            section.UpdateName(command.Name);
            section.UpdateDescription(command.Description);
            section.UpdateGoalId(command.GoalId);
            
            await sectionRepository.UpdateAsync(section, cancellationToken);
        });
}