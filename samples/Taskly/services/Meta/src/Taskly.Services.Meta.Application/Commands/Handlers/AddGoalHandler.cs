using Taskly.Services.Meta.Domain.Entities;
using Taskly.Services.Meta.Domain.Repositories;

namespace Taskly.Services.Meta.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class AddGoalHandler(IContextProvider contextProvider, IGoalRepository goalRepository)
    : ICommandHandler<AddGoal, Guid>
{
    public async Task<Guid> HandleAsync(AddGoal command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var goal = new Goal(Guid.NewGuid(), command.Name, Guid.Parse(context.UserId!), command.Description);
        await goalRepository.AddAsync(goal, cancellationToken);
        return goal.Id;
    }
}