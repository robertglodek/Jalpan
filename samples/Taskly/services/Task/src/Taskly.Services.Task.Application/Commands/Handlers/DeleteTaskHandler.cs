using Jalpan;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using JetBrains.Annotations;
using Taskly.Services.Task.Application.Exceptions;
using Taskly.Services.Task.Domain.Repositories;

namespace Taskly.Services.Task.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class DeleteTaskHandler(IContextProvider contextProvider, ITaskRepository taskRepository)
    : ICommandHandler<DeleteTask, Empty>
{
    public async Task<Empty> HandleAsync(DeleteTask command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var task = await taskRepository.GetAsync(command.Id, cancellationToken) ?? throw new TaskNotFoundException(command.Id);
            if (task.UserId != Guid.Parse(context.UserId!))
            {
                throw new UnauthorizedTaskAccessException(task.Id, Guid.Parse(context.UserId!));
            }

            await taskRepository.DeleteAsync(command.Id, cancellationToken);
        });
}
