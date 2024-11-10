using Jalpan.Exceptions;

namespace Jalpan.Discovery.Consul.Exceptions;

public sealed class ConsulConfigurationException(string message) : CustomException(message);