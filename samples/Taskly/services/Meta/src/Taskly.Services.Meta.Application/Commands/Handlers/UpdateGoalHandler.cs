using Taskly.Services.Meta.Application.Exceptions;
using Taskly.Services.Meta.Domain.Repositories;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class UpdateGoalHandler(IContextProvider contextProvider, IGoalRepository goalRepository)
    : ICommandHandler<UpdateGoal, Empty>
{
    public async Task<Empty> HandleAsync(UpdateGoal command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var goal = await goalRepository.GetAsync(command.Id, cancellationToken);
            if (goal == null)
            {
                throw new GoalNotFoundException(command.Id);
            }

            if (goal.UserId != Guid.Parse(context.UserId!))
            {
                throw new UnauthorizedGoalAccessException(goal.Id, Guid.Parse(context.UserId!));
            }
            
            goal.UpdateName(command.Name);
            goal.UpdateDescription(command.Description);
            
            await goalRepository.UpdateAsync(goal, cancellationToken);
        });
}