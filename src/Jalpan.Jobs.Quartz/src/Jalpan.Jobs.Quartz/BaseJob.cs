using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jalpan.Jobs.Quartz;

public abstract class BaseJob(ILogger logger, IDateTime dateTime) : IJob
{
    protected readonly ILogger _logger = logger;
    protected readonly IDateTime _dateTime = dateTime;

    protected virtual int MaxRetryCount => 0;
    protected virtual int RetryIntervalInSeconds => 30;
    protected virtual TimeSpan LockTimeout => TimeSpan.FromMinutes(1);

    public async Task Execute(IJobExecutionContext context)
    {
        var jobType = context.JobDetail.JobType;
        var jobName = jobType.Name;
        var retryCount = context.JobDetail.JobDataMap.GetInt("RetryCount");

        try
        {
            _logger.LogInformation("Job {JobName} started at {StartTime}", jobName, _dateTime.Now);
            await ExecuteJob(context); 
            _logger.LogInformation("Job {JobName} finished at {EndTime}", jobName, _dateTime.Now);
            context.JobDetail.JobDataMap.Put("RetryCount", 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job {JobName} failed on attempt {RetryCount} at {ErrorTime} with error: {ErrorMessage}", jobName, retryCount, _dateTime.Now, ex.Message);
            retryCount++;
            context.JobDetail.JobDataMap.Put("RetryCount", retryCount);

            if (retryCount <= MaxRetryCount)
            {
                ScheduleRetry(context, ex);
            }
            else
            {
                _logger.LogCritical("Max retry attempts reached. Job will not be retried.");
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
