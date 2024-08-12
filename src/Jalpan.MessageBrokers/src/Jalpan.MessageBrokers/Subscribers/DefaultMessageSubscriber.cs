using Jalpan.Types;

namespace Jalpan.Messaging.Subscribers;

internal sealed class DefaultMessageSubscriber : IMessageSubscriber
{
    public IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler) where T : class, IMessage => this;

    public IMessageSubscriber Command<T>() where T : class, ICommand<Empty> => this;

    public IMessageSubscriber Event<T>() where T : class, IEvent => this;
}