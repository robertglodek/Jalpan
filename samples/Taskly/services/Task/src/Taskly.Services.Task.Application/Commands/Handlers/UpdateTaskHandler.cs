using Jalpan;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Time;
using JetBrains.Annotations;
using Taskly.Services.Task.Application.Exceptions;
using Taskly.Services.Task.Domain.Entities;
using Taskly.Services.Task.Domain.Repositories;

namespace Taskly.Services.Task.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class UpdateTaskHandler(IContextProvider contextProvider, ITaskRepository takRepository, IDateTime dateTime)
    : ICommandHandler<UpdateTask, Empty>
{
    public async Task<Empty> HandleAsync(UpdateTask command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var task = await takRepository.GetAsync(command.Id, cancellationToken) ?? throw new TaskNotFoundException(command.Id);

            if (task.UserId != Guid.Parse(context.UserId!))
            {
                throw new UnauthorizedTaskAccessException(task.Id, Guid.Parse(context.UserId!));
            }

            TaskInterval? interval = command.Interval is not null
            ? new TaskInterval(
                command.Interval.Interval,
                command.Interval.StartDate,
                command.Interval.Hour)
            : null;

            task.Description = command.Description;
            task.Active = command.Active;
            task.Repeatable = command.Repeatable;
            task.PriorityLevel = command.PriorityLevel;
            task.Tags = command.Tags!;
            task.GoalId = command.GoalId;
            task.SectionId = command.SectionId;
            task.UpdateName(command.Name);
            task.RootTaskId = command.RootTaskId;
            task.UpdateIntervalAndDueDate(interval, command.DueDate);
            task.ModifiedAt = dateTime.Now;

            await takRepository.UpdateAsync(task, cancellationToken);
        });

}
