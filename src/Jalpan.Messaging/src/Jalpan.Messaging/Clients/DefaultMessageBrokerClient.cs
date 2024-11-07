using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Messaging.Clients;

internal sealed class DefaultMessageBrokerClient(ILogger<DefaultMessageBrokerClient> logger) : IMessageBrokerClient
{
    public Task SendAsync<T>(MessageEnvelope<T> message, CancellationToken cancellationToken = default)
        where T : IMessage
    {
        var name = message.Message.GetType().Name.Underscore();
        logger.LogInformation("Default message broker, message: '{Name}', ID: '{MessageId}' won't be sent.", name, message.Context.MessageId);
        return Task.CompletedTask;
    }
}