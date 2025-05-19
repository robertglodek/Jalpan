using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Taskly.Services.Notification.Domain.Repositories;

namespace Taskly.Services.Notification.Application.Commands.Handlers;

public sealed class CreateNotificationActionHandler(IContextProvider contextProvider,
    INotificationScheduleRepository notificationRepository,
    IDateTime dateTime,
    ILogger<CreateNotificationActionHandler> logger) : ICommandHandler<CreateNotificationAction, Guid>
{
    public async Task<Guid> HandleAsync(CreateNotificationAction command, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        var schedule = new Domain.Entities.Schedule(command.Schedule.ReminderTiming, command.Schedule.ScheduleFrequency, command.Schedule.CustomOffsetValue, command.Schedule.SendTime);
        var notification = new Domain.Entities.Notification(Guid.NewGuid(), dateTime.Now, null,
            Guid.Parse(context.UserId!), command.Name, command.Search, command.Tags, command.SectionId, command.GoalId, schedule);
        await notificationRepository.AddAsync(notification, cancellationToken);

        logger.LogInformation("Added new notification for user with id: {UserId}", context.UserId);

        return notification.Id;
    }
}
