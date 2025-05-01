using Quartz;
using Taskly.Services.Notification.Application.Services;
using Taskly.Services.Notification.Domain.Enums;
using Taskly.Services.Notification.Infrastructure.Quartz.Jobs;

namespace Taskly.Services.Notification.Infrastructure.Quartz;

internal sealed class UserTaskNotificationJobManager(IScheduler scheduler) : IUserTaskNotificationJobManager
{
    public async Task ScheduleNotificationJobAsync(Guid notificationId, ScheduleFrequency scheduleFrequency, TimeSpan sendTime)
    {
        var jobName = nameof(UserTaskNotificationJob);
        var jobKey = new JobKey($"{jobName}-{notificationId}");
        var triggerKey = new TriggerKey($"{jobName}-{notificationId}-trigger");

        var job = JobBuilder.Create<UserTaskNotificationJob>()
            .WithIdentity(jobKey)
            .UsingJobData("NotificationId", notificationId)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithCronSchedule(CronExpressionBuilder.Generate(scheduleFrequency, sendTime))
            .ForJob(jobKey)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task RemoveNotificationJobAsync(Guid notificationId)
    {
        var jobKey = new JobKey($"{nameof(UserTaskNotificationJob)}-{notificationId}");
        await scheduler.DeleteJob(jobKey);
    }

    public async Task RescheduleNotificationJobAsync(Guid notificationId, ScheduleFrequency scheduleFrequency, TimeSpan sendTime)
    {
        var jobName = nameof(UserTaskNotificationJob);
        var jobKey = new JobKey($"{jobName}-{notificationId}");
        var triggerKey = new TriggerKey($"{jobName}-{notificationId}-trigger");

        // Check if job exists
        if (!await scheduler.CheckExists(jobKey))
            throw new InvalidOperationException($"No job found with ID: {notificationId}");

        // Build new trigger
        var newTrigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithCronSchedule(CronExpressionBuilder.Generate(scheduleFrequency, sendTime))
            .ForJob(jobKey)
            .Build();

        // Reschedule trigger
        await scheduler.RescheduleJob(triggerKey, newTrigger);
    }
}
