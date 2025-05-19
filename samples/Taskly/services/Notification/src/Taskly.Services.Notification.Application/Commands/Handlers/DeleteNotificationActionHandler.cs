using Jalpan;
using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Microsoft.Extensions.Logging;
using Taskly.Services.Notification.Application.Exceptions;
using Taskly.Services.Notification.Domain.Repositories;

namespace Taskly.Services.Notification.Application.Commands.Handlers;

public sealed class DeleteNotificationHandler(IContextProvider contextProvider,
    INotificationScheduleRepository notificationRepository,
    ILogger<DeleteNotificationHandler> logger) : ICommandHandler<DeleteNotification, Empty>
{
    public async Task<Empty> HandleAsync(DeleteNotification command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var notification = await notificationRepository.GetAsync(command.Id) ?? throw new NotificationNotFoundException(command.Id);

            await notificationRepository.DeleteAsync(command.Id, cancellationToken);

            logger.LogInformation("Deleted notification with id: {NotificationId} for user with id: {UserId}", command.Id, context.UserId);
        });
}
