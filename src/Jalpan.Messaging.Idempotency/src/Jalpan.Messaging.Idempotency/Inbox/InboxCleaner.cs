using Jalpan.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Jalpan.Messaging.Idempotency.Inbox;

internal sealed class InboxCleaner(
    IServiceProvider serviceProvider,
    IOptions<InboxOptions> options, IDateTime dateTime,
    ILogger<InboxCleaner> logger) : BackgroundService
{
    private readonly TimeSpan _interval = options.Value.CleanupInterval ?? TimeSpan.FromHours(1);
    private readonly bool _enabled = options.Value.Enabled;
    private int _isProcessing;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_interval, stoppingToken);
                continue;
            }

            logger.LogInformation("Started cleaning up inbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                try
                {
                    var inbox = scope.ServiceProvider.GetRequiredService<IInbox>();
                    await inbox.CleanupAsync(dateTime.Now.Subtract(_interval), stoppingToken);
                }
                catch (Exception exception)
                {
                    logger.LogError("There was an error when processing inbox.");
                    logger.LogError(exception, "An error occurred: {Message}", exception.Message);

                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopwatch.Stop();
                    logger.LogInformation("Finished cleaning up inbox messages in {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}