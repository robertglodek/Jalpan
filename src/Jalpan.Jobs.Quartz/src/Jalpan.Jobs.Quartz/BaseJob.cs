using Jalpan.Time;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jalpan.Jobs.Quartz;

public abstract class BaseJob : IJob
{
    protected readonly ILogger _logger;
    protected readonly IDateTime _dateTime;
    private readonly IDistributedCache? _distributedCache;

    protected BaseJob(ILogger logger, IDateTime dateTime, IDistributedCache? distributedCache = null)
    {
        _logger = logger;
        _dateTime = dateTime;
        _distributedCache = distributedCache;
    }

    protected virtual int MaxRetryCount => 0;
    protected virtual int RetryIntervalInSeconds => 30;
    protected virtual TimeSpan LockTimeout => TimeSpan.FromMinutes(1);

    public async Task Execute(IJobExecutionContext context)
    {
        var jobType = context.JobDetail.JobType;
        var jobName = jobType.Name;
        var retryCount = context.JobDetail.JobDataMap.GetInt("RetryCount");
        bool disallowConcurrent = IsDisallowConcurrentExecution(jobType);
        bool lockAcquired = false;

        if (disallowConcurrent && _distributedCache != null)
        {
            _logger.LogWarning("Job {JobName} has both [DisallowConcurrentExecution] attribute and cashe lock defined. This configuration is redundant and may cause unexpected behavior.", jobType.FullName);
            throw new InvalidOperationException($"Job {jobType.FullName} is configured with both [DisallowConcurrentExecution] and cashe locking. This configuration is conflicting.");
        }

        if (_distributedCache != null)
        {
            var lockKey = $"{context.JobDetail.Key.Name}-lock";
            lockAcquired = await AcquireLockAsync(lockKey, LockTimeout);

            if (!lockAcquired)
            {
                _logger.LogInformation("Job execution skipped because the lock could not be acquired.");
                return; // Exit if the lock could not be acquired
            }
        }

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
        finally
        {
            if (_distributedCache != null && lockAcquired)
            {
                await ReleaseLockAsync($"{context.JobDetail.Key.Name}-lock");
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

    private bool IsDisallowConcurrentExecution(Type jobType)
        => Attribute.IsDefined(jobType, typeof(DisallowConcurrentExecutionAttribute));

    protected abstract Task ExecuteJob(IJobExecutionContext context);

    private async Task<bool> AcquireLockAsync(string lockKey, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var lockValue = Guid.NewGuid().ToString();

        var lockAcquired = await _distributedCache!.GetStringAsync(lockKey, cancellationToken) == null;

        if (lockAcquired)
        {
            await _distributedCache!.SetStringAsync(lockKey, lockValue,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeout }, cancellationToken);

            _logger.LogInformation("Lock acquired with key {LockKey} and value {LockValue}", lockKey, lockValue);
            return true;
        }

        _logger.LogWarning("Failed to acquire lock with key {LockKey} because it already exists.", lockKey);
        return false;
    }

    private async Task ReleaseLockAsync(string lockKey, CancellationToken cancellationToken = default)
    {
        var lockValue = await _distributedCache!.GetStringAsync(lockKey, cancellationToken);

        if (!string.IsNullOrEmpty(lockValue))
        {
            await _distributedCache!.RemoveAsync(lockKey, cancellationToken);
            _logger.LogInformation("Lock released with key {LockKey} and token {LockValue}", lockKey, lockValue);
        }
    }
}
