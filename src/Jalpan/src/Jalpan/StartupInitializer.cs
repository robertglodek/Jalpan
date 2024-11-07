using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jalpan;

internal sealed class StartupInitializer(IServiceProvider serviceProvider, ILogger<StartupInitializer> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var initializers = scope.ServiceProvider.GetServices<IInitializer>();
        foreach (var initializer in initializers)
        {
            try
            {
                logger.LogInformation("Running the initializer: {InitializerType}", initializer.GetType().Name);
                await initializer.InitializeAsync();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while processing: {ErrorMessage}", exception.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
