using Jalpan;
using Jalpan.Handlers;
using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Taskly.Services.Report.Domain.Entities;
using Taskly.Services.Report.Domain.Repositories;

namespace Taskly.Services.Report.Application.Events.External.Handlers;

internal sealed class TaskFinishedHandler(
    ITaskHistoryRepository taskHistoryRepository,
    ILogger<TaskFinishedHandler> logger,
    IDateTime dateTime) : IEventHandler<TaskFinished>
{
    public async Task HandleAsync(TaskFinished @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Received '{EventName}' event with task id: {TaskId}", nameof(TaskFinished).Underscore(), @event.Id);

        var taskHistory = new TaskHistory(Guid.NewGuid(), @event.UserId, @event.Id, @event.Name, @event.Success, @event.DateTime, dateTime.Now);
        await taskHistoryRepository.AddAsync(taskHistory, cancellationToken);
    }
}
