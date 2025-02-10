using Taskly.Services.Meta.Application.Exceptions;
using Taskly.Services.Meta.Domain.Repositories;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class DeleteGoalHandler(IContextProvider contextProvider, IGoalRepository goalRepository)
    : ICommandHandler<DeleteGoal, Empty>
{
    public async Task<Empty> HandleAsync(DeleteGoal command, CancellationToken cancellationToken = default)
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

            await goalRepository.DeleteAsync(command.Id, cancellationToken);
        });
}