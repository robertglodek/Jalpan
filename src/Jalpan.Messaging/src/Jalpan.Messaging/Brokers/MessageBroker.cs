using Jalpan.Contexts;
using Jalpan.Contexts.Providers;
using Jalpan.Messaging.Clients;
using Jalpan.Types;
namespace Jalpan.Messaging.Brokers;

internal sealed class MessageBroker(IMessageBrokerClient client, IContextProvider contextProvider)
    : IMessageBroker
{
    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        var messageId = Guid.NewGuid().ToString("N");
        var context = contextProvider.Current();
        await client.SendAsync(new MessageEnvelope<T>(message, new MessageContext(messageId, context)), cancellationToken);
    }
}