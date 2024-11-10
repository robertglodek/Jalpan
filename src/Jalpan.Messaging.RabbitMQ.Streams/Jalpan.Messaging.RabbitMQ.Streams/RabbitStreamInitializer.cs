using Microsoft.Extensions.Hosting;

namespace Jalpan.Messaging.RabbitMQ.Streams;

internal sealed class RabbitStreamInitializer(RabbitStreamManager streamManager) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => streamManager.InitAsync();

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}