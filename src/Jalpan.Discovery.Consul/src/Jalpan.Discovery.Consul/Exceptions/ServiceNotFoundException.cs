using Jalpan.Exceptions;

namespace Jalpan.Discovery.Consul.Exceptions;

public sealed class ServiceNotFoundException(string service) : CustomException($"Service: '{service}' was not found.");