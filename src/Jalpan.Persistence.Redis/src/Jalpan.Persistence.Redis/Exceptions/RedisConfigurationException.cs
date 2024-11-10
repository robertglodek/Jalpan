using Jalpan.Exceptions;

namespace Jalpan.Persistence.Redis.Exceptions;

public sealed class RedisConfigurationException(string message) : CustomException(message);
