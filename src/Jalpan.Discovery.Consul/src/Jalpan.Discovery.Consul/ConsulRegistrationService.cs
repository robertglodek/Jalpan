using System.Net;
using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jalpan.Discovery.Consul;

public class ConsulRegistrationService(IConsulClient client, ILogger<ConsulRegistrationService> logger,
    AgentServiceRegistration agentServiceRegistration) : IHostedService
{
    private readonly IConsulClient _client = client;
    private readonly ILogger<ConsulRegistrationService> _logger = logger;
    private readonly AgentServiceRegistration _agentServiceRegistration = agentServiceRegistration;
    private readonly string _serviceIdentifier = agentServiceRegistration.ID;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Registering a service: '{_serviceIdentifier}' in Consul...");
        var result = await _client.Agent.ServiceRegister(_agentServiceRegistration, cancellationToken);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation($"Registered a service: '{_serviceIdentifier}' in Consul.");
            return;
        }
        
        _logger.LogError($"There was an error: {result.StatusCode} when registering a service: '{_serviceIdentifier}' in Consul.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deregistering a service: '{_serviceIdentifier}' in Consul...");
        var result = await _client.Agent.ServiceDeregister(_serviceIdentifier, cancellationToken);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation($"Deregistered a service: '{_serviceIdentifier}' in Consul.");
            return;
        }
        
        _logger.LogError($"There was an error: {result.StatusCode} when deregistering a service: '{_serviceIdentifier}' in Consul.");
    }
}