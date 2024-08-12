using Jalpan.Exceptions;

namespace Jalpan.Discovery.Consul.Exceptions;

internal sealed class ServiceNotFoundException : CustomException
{
    public string Service { get; }

    public ServiceNotFoundException(string service) : base($"Service: '{service}' was not found.")
    {
        Service = service;
    }
}