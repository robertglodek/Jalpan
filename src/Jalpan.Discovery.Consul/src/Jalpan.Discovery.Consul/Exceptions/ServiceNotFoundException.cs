using Jalpan.Exceptions;

namespace Jalpan.Discovery.Consul.Exceptions;

internal sealed class ServiceNotFoundException(string service)
    : CustomException($"Service: '{service}' was not found.");