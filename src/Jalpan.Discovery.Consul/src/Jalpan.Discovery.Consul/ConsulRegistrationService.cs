using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jalpan.Discovery.Consul;

public class ConsulRegistrationService(
    IConsulClient client,
    ILogger<ConsulRegistrationService> logger,
    AgentServiceRegistration agentServiceRegistration) : IHostedService
{
    private readonly string _serviceIdentifier = agentServiceRegistration.ID;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering a service: '{ServiceIdentifier}' in Consul...", _serviceIdentifier);
        var result = await client.Agent.ServiceRegister(agentServiceRegistration, cancellationToken);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            logger.LogInformation("Registered a service: '{ServiceIdentifier}' in Consul.", _serviceIdentifier);
            return;
        }
        
        logger.LogError("There was an error: {StatusCode} when registering a service: '{ServiceIdentifier}' in Consul.",
            result.StatusCode, _serviceIdentifier);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Deregistering a service: '{ServiceIdentifier}' in Consul...", _serviceIdentifier);
        var result = await client.Agent.ServiceDeregister(_serviceIdentifier, cancellationToken);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            logger.LogInformation("Deregistered a service: '{ServiceIdentifier}' in Consul.", _serviceIdentifier);
            return;
        }
        
        logger.LogError("There was an error: {StatusCode} when deregistering a service: '{ServiceIdentifier}' in Consul.",
            result.StatusCode, _serviceIdentifier);
    }
}