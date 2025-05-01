using Jalpan.Jobs.Quartz;
using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Quartz;
using Taskly.Services.Notification.Application.Services;

namespace Taskly.Services.Notification.Infrastructure.Quartz.Jobs;

internal class UserTaskNotificationJob(ILogger<UserTaskNotificationJob> logger, IDateTime dateTime, IUserTaskNotificationLogicHandler notificationHandler) : BaseJob(logger, dateTime)
{
    protected override async Task ExecuteJob(IJobExecutionContext context)
    {
        if (!context.MergedJobDataMap.TryGetValue("NotificationId", out var notificationIdObj) || notificationIdObj is not Guid notificationId)
        {
            Logger.LogError("NotificationId not found or invalid in job data.");
            return;
        }
        await notificationHandler.HandleAsync(notificationId);
    }
}