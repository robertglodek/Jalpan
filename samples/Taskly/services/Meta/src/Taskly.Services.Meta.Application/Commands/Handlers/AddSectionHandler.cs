using Taskly.Services.Meta.Application.Exceptions;
using Taskly.Services.Meta.Domain.Entities;
using Taskly.Services.Meta.Domain.Repositories;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class AddSectionHandler(
    IContextProvider contextProvider,
    IGoalRepository goalRepository,
    ISectionRepository sectionRepository)
    : ICommandHandler<AddSection, Guid>
{
    public async Task<Guid> HandleAsync(AddSection command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        if (command.GoalId.HasValue && await goalRepository.GetAsync(command.GoalId.Value, cancellationToken) == null)
        {
            throw new GoalNotFoundException(command.GoalId.Value);
        }
        
        var section = new Section(Guid.NewGuid(), command.Name, Guid.Parse(context.UserId!), command.Description, command.GoalId);
        await sectionRepository.AddAsync(section, cancellationToken);
        return section.Id;
    }
}