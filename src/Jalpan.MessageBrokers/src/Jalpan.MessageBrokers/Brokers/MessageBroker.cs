using Jalpan.Contexts;
using Jalpan.Contexts.Providers;
using Jalpan.Messaging.Clients;
using Jalpan.Types;
namespace Jalpan.Messaging.Brokers;

internal sealed class MessageBroker(IMessageBrokerClient client, IContextProvider contextProvider) : IMessageBroker
{
    private readonly IMessageBrokerClient _client = client;
    private readonly IContextProvider _contextProvider = contextProvider;

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        var messageId = Guid.NewGuid().ToString("N");
        var context = _contextProvider.Current();
        await _client.SendAsync(new MessageEnvelope<T>(message, new MessageContext(messageId, context)), cancellationToken);
    }
}