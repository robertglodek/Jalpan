using Jalpan;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Taskly.Services.Notification.Application.Commands;
using Taskly.Services.Notification.Application.Exceptions;
using Taskly.Services.Notification.Domain.Repositories;


public sealed class UpdateNotificationHandler(IContextProvider contextProvider,
    INotificationRepository notificationRepository,
    IDateTime dateTime,
    ILogger<UpdateNotificationHandler> logger) : ICommandHandler<UpdateNotification, Empty>
{
    public async Task<Empty> HandleAsync(UpdateNotification command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var notification = await notificationRepository.GetAsync(command.Id) ?? throw new NotificationNotFoundException(command.Id);

            notification.Search = command.Search;
            notification.UpdateName(command.Name);
            notification.Tags = command.Tags;
            notification.SectionId = command.SectionId;
            notification.GoalId = command.GoalId;
            notification.Schedule.CustomOffsetValue = command.Schedule.CustomOffsetValue;
            notification.Schedule.ReminderTiming = command.Schedule.ReminderTiming;
            notification.Schedule.SendTime = command.Schedule.SendTime;
            notification.LastModifiedAt = dateTime.Now;

            await notificationRepository.UpdateAsync(notification, cancellationToken);

            logger.LogInformation("Updated notification with id: {NotificationId} for user with id: {UserId}", command.Id ,context.UserId);
        });
}