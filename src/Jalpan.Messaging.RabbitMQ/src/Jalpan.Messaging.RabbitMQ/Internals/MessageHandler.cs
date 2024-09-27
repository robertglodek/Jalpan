using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Messaging.RabbitMQ.Internals;

internal sealed class MessageHandler(IServiceProvider serviceProvider, ILogger<MessageHandler> logger) : IMessageHandler
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<MessageHandler> _logger = logger;

    public async Task HandleAsync<T>(Func<IServiceProvider, T, CancellationToken, Task> handler, T message,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        try
        {
            await handler(_serviceProvider, message, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw;
        }
    }
}