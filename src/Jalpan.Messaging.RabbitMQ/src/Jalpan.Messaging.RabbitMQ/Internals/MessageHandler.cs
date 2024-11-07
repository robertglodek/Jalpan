using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Messaging.RabbitMQ.Internals;

internal sealed class MessageHandler(IServiceProvider serviceProvider, ILogger<MessageHandler> logger)
    : IMessageHandler
{
    public async Task HandleAsync<T>(Func<IServiceProvider, T, CancellationToken, Task> handler, T message,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        try
        {
            await handler(serviceProvider, message, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred: {Message}", exception.Message);
            throw;
        }
    }
}