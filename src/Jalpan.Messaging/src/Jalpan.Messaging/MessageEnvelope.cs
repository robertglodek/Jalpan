using Jalpan.Contexts;
using Jalpan.Types;

namespace Jalpan.Messaging;

public sealed record MessageEnvelope<T>(T Message, MessageContext Context) where T : IMessage;