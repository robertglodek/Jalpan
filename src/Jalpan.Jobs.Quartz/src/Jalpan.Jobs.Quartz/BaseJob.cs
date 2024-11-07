using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jalpan.Jobs.Quartz;

public abstract class BaseJob(ILogger logger, IDateTime dateTime) : IJob
{
    protected readonly ILogger Logger = logger;
    protected readonly IDateTime DateTime = dateTime;

    protected virtual int MaxRetryCount => 0;
    protected virtual int RetryIntervalInSeconds => 30;

    public async Task Execute(IJobExecutionContext context)
    {
        var jobType = context.JobDetail.JobType;
        var jobName = jobType.Name;
        var retryCount = context.JobDetail.JobDataMap.GetInt("RetryCount");

        try
        {
            Logger.LogInformation("Job {JobName} started at {StartTime}", jobName, DateTime.Now);
            await ExecuteJob(context); 
            Logger.LogInformation("Job {JobName} finished at {EndTime}", jobName, DateTime.Now);
            context.JobDetail.JobDataMap.Put("RetryCount", 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Job {JobName} failed on attempt {RetryCount} at {ErrorTime} with error: {ErrorMessage}", jobName, retryCount, DateTime.Now, ex.Message);
            retryCount++;
            context.JobDetail.JobDataMap.Put("RetryCount", retryCount);

            if (retryCount <= MaxRetryCount)
            {
                ScheduleRetry(context, ex);
            }
            else
            {
                Logger.LogCritical("Max retry attempts reached. Job will not be retried.");
                HandleMaxRetriesExceeded(context, ex);
            }
        }
    }

    private void ScheduleRetry(IJobExecutionContext context, Exception ex)
    {
        var scheduler = context.Scheduler;

        var retryTrigger = TriggerBuilder.Create()
            .ForJob(context.JobDetail)
            .WithIdentity($"{context.JobDetail.Key.Name}-retry")
            .StartAt(DateBuilder.FutureDate(RetryIntervalInSeconds, IntervalUnit.Second))
            .Build();

        context.JobDetail.JobDataMap.Put("LastException", ex); // Store the exception
        scheduler.ScheduleJob(retryTrigger);
    }

    protected virtual void HandleMaxRetriesExceeded(IJobExecutionContext context, Exception ex)
    {
        // Handle the situation where the max number of retries is exceeded
        // E.g., send notifications, perform clean-up, etc.
    }

    protected abstract Task ExecuteJob(IJobExecutionContext context);
}
