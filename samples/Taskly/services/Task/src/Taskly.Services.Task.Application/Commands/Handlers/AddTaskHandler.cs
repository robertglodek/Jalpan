using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using JetBrains.Annotations;
using Taskly.Services.Task.Domain.Entities;
using Taskly.Services.Task.Domain.Repositories;

namespace Taskly.Services.Task.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class AddTaskHandler(IContextProvider contextProvider, ITaskRepository taskRepository)
    : ICommandHandler<AddTask, Guid>
{
    public async Task<Guid> HandleAsync(AddTask command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        TaskInterval? interval = command.Interval is not null
            ? new TaskInterval(
                command.Interval.Interval,
                command.Interval.StartDate,
                command.Interval.Hour)
            : null;

        var task = new Domain.Entities.Task(
            Guid.NewGuid(),
            Guid.Parse(context.UserId!),
            command.Name,
            command.Description,
            command.SectionId,
            command.GoalId,
            command.RootTaskId,
            command.PriorityLevel,
            command.Tags,
            command.Repeatable,
            command.Active,
            interval,
            command.DueDate
        );
        await taskRepository.AddAsync(task, cancellationToken);
        return task.Id;
    }
}
