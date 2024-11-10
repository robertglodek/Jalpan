using Jalpan.Exceptions;

namespace Jalpan.LoadBalancing.Fabio.Exceptions;

public sealed class FabioConfigurationException(string message) : CustomException(message);