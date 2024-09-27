using Microsoft.Extensions.Hosting;

namespace Jalpan.Messaging.RabbitMQ.Streams;

internal sealed class RabbitStreamInitializer(RabbitStreamManager streamManager) : IHostedService
{
    private readonly RabbitStreamManager _streamManager = streamManager;

    public Task StartAsync(CancellationToken cancellationToken) => _streamManager.InitAsync();

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}