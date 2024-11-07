using Jalpan.Secrets.Vault.Issuers;
using Jalpan.Secrets.Vault.Stores;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VaultSharp;

namespace Jalpan.Secrets.Vault.Services;

internal sealed class VaultHostedService(
    IVaultClient client,
    ILeaseService leaseService,
    ICertificatesIssuer certificatesIssuer,
    ICertificatesStore certificatesStore,
    IOptions<VaultOptions> options,
    ILogger<VaultHostedService> logger) : BackgroundService
{
    private readonly VaultOptions _options = options.Value;
    private readonly int _interval = options.Value.RenewalsInterval <= 0 ? 10 : options.Value.RenewalsInterval;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        if (!_options.Pki.Enabled && _options.Lease.All(l => !l.Value.Enabled) ||
             !_options.Lease.Any(l => l.Value.AutoRenewal))
        {
            return;
        }

        logger.LogInformation("Vault lease renewals will be processed every {Interval} seconds.", _interval);
        var interval = TimeSpan.FromSeconds(_interval);
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextIterationAt = now.AddSeconds(2 * _interval);

            if (_options.Pki is not null && _options.Pki.Enabled)
            {
                foreach (var (role, cert) in certificatesStore.All)
                {
                    if (cert.NotAfter > nextIterationAt)
                    {
                        continue;
                    }

                    logger.LogInformation("Issuing a certificate for: '{Role}'.", role);
                    var certificate = await certificatesIssuer.IssueAsync();
                    if (certificate is not null)
                    {
                        certificatesStore.Set(role, certificate);
                    }
                }
            }

            foreach (var (key, lease) in leaseService.All.Where(l => l.Value.AutoRenewal))
            {
                if (lease.ExpiryAt > nextIterationAt)
                {
                    continue;
                }

                logger.LogInformation("Renewing a lease with ID: '{LeaseId}', for: '{Key}', duration: {Duration} seconds.", lease.Id, key, lease.Duration);

                var beforeRenew = DateTime.UtcNow;
                var renewedLease = await client.V1.System.RenewLeaseAsync(lease.Id, lease.Duration);
                lease.Refresh(renewedLease.LeaseDurationSeconds - (lease.ExpiryAt - beforeRenew).TotalSeconds);
            }

            await Task.Delay(interval.Subtract(DateTime.UtcNow - now), stoppingToken);
        }

        if (!_options.RevokeLeaseOnShutdown)
        {
            return;
        }

        foreach (var (key, lease) in leaseService.All)
        {
            logger.LogInformation("Revoking a lease with ID: '{LeaseId}', for: '{Key}'.", lease.Id, key);
            await client.V1.System.RevokeLeaseAsync(lease.Id);
        }
    }
}