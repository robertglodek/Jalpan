using Jalpan.Types;
using Microsoft.Extensions.Logging;

namespace Jalpan.Messaging.Clients;

internal sealed class DefaultMessageBrokerClient(ILogger<DefaultMessageBrokerClient> logger) : IMessageBrokerClient
{
    private readonly ILogger<DefaultMessageBrokerClient> _logger = logger;

    public Task SendAsync<T>(MessageEnvelope<T> message, CancellationToken cancellationToken = default)
        where T : IMessage
    {
        var name = message.Message.GetType().Name.Underscore();
        _logger.LogInformation($"Default message broker, message: '{name}', ID: '{message.Context.MessageId}' won't be sent.");
        return Task.CompletedTask;
    }
}