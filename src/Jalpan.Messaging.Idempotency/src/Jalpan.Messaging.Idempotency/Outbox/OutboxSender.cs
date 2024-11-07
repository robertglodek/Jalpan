using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jalpan.Messaging.Idempotency.Outbox;

internal sealed class OutboxSender(
    IServiceProvider serviceProvider,
    IOptions<OutboxOptions> options, ILogger<OutboxSender> logger) : BackgroundService
{
    private readonly TimeSpan _senderInterval = options.Value.SenderInterval ?? TimeSpan.FromSeconds(5);
    private readonly bool _enabled = options.Value.Enabled;
    private int _isProcessing;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            logger.LogWarning("Outbox is disabled");
            return;
        }

        logger.LogInformation("Outbox is enabled, sender interval: {SenderInterval}", _senderInterval);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_senderInterval, stoppingToken);
                continue;
            }
                
            logger.LogInformation("Started processing outbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                try
                {
                    var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                    await outbox.PublishUnsentAsync(stoppingToken);
                }
                catch (Exception exception)
                {
                    logger.LogError("There was an error when processing outbox.");
                    logger.LogError("An error occurred: {Message}", exception.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopwatch.Stop();
                    logger.LogInformation("Finished processing outbox messages in {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
                }
            }

            await Task.Delay(_senderInterval, stoppingToken);
        }
    }
}