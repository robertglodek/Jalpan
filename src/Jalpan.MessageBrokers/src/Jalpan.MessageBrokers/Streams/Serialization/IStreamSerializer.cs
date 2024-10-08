using Jalpan.Types;

namespace Jalpan.Messaging.Streams.Serialization;

public interface IStreamSerializer
{
    byte[] Serialize<T>(T message) where T : IMessage;
    T? Deserialize<T>(byte[] bytes) where T : IMessage;
}