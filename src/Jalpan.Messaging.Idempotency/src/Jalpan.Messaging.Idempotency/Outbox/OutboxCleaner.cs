using System.Diagnostics;
using Jalpan.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jalpan.Messaging.Idempotency.Outbox;

internal sealed class OutboxCleaner(IServiceProvider serviceProvider, IOptions<OutboxOptions> options, IDateTime dateTime,
    ILogger<OutboxCleaner> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IDateTime _dateTime = dateTime;
    private readonly ILogger<OutboxCleaner> _logger = logger;
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

            _logger.LogInformation("Started cleaning up outbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                try
                {
                    var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                    await outbox.CleanupAsync(_dateTime.Now.Subtract(_interval), stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError("There was an error when processing outbox.");
                    _logger.LogError(exception, exception.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopwatch.Stop();
                    _logger.LogInformation($"Finished cleaning up outbox messages in {stopwatch.ElapsedMilliseconds} ms.");
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}