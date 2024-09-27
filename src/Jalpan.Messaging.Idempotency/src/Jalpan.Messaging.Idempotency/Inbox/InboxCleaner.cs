using Jalpan.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Jalpan.Messaging.Idempotency.Inbox;

internal sealed class InboxCleaner(IServiceProvider serviceProvider, IOptions<InboxOptions> options, IDateTime dateTime,
    ILogger<InboxCleaner> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IDateTime _dateTime = dateTime;
    private readonly ILogger<InboxCleaner> _logger = logger;
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

            _logger.LogInformation("Started cleaning up inbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                try
                {
                    var inbox = scope.ServiceProvider.GetRequiredService<IInbox>();
                    await inbox.CleanupAsync(_dateTime.Now.Subtract(_interval), stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError("There was an error when processing inbox.");
                    _logger.LogError(exception, exception.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopwatch.Stop();
                    _logger.LogInformation($"Finished cleaning up inbox messages in {stopwatch.ElapsedMilliseconds} ms.");
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}