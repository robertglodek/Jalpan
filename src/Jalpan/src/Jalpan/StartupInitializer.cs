using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jalpan;

internal sealed class StartupInitializer(IServiceProvider serviceProvider, ILogger<StartupInitializer> logger) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<StartupInitializer> _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var initializers = scope.ServiceProvider.GetServices<IInitializer>();
        foreach (var initializer in initializers)
        {
            try
            {
                _logger.LogInformation("Running the initializer: {InitializerType}", initializer.GetType().Name);
                await initializer.InitializeAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing: {ErrorMessage}", exception.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
