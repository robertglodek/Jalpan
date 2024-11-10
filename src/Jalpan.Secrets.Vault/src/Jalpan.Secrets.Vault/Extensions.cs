using Jalpan.Secrets.Vault.Exceptions;
using Jalpan.Secrets.Vault.Issuers;
using Jalpan.Secrets.Vault.Secrets;
using Jalpan.Secrets.Vault.Services;
using Jalpan.Secrets.Vault.Stores;
using Jalpan.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp.V1.SecretsEngines;

namespace Jalpan.Secrets.Vault;

public static class Extensions
{
    private const string DefaultSectionName = "vault";
    private const string RegistryKey = "secrets.valut";
    private static readonly LeaseService LeaseService = new();
    private static readonly CertificatesStore CertificatesStore = new();
    private static readonly string[] EngineVersions = ["V1", "V2"];

    public static IHostBuilder UseVault(this IHostBuilder builder, string sectionName = DefaultSectionName)
        => builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

            var options = cfg.Build().BindOptions<VaultOptions>(sectionName);
            if (!options.Enabled)
            {
                return;
            }
            cfg.AddVaultAsync(options).GetAwaiter().GetResult();
        });

    public static IJalpanBuilder AddVault(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<VaultOptions>();
        builder.Services.Configure<VaultOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        VerifyEngineVersion(options);
        RegisterServices(builder, options);

        return builder;
    }

    private static void RegisterServices(IJalpanBuilder builder, VaultOptions options)
    {
        var (client, settings) = GetClientAndSettings(options);
        builder.Services.AddTransient<IKeyValueSecrets, KeyValueSecrets>();
        builder.Services.AddSingleton(settings);
        builder.Services.AddSingleton(client);
        builder.Services.AddSingleton(LeaseService);
        builder.Services.AddSingleton(CertificatesStore);
        builder.Services.AddHostedService<VaultHostedService>();

        if (options.Pki.Enabled)
        {
            builder.Services.AddSingleton<ICertificatesIssuer, CertificatesIssuer>();
        }
        else
        {
            builder.Services.AddSingleton<ICertificatesIssuer, EmptyCertificatesIssuer>();
        }
    }

    private static void VerifyEngineVersion(VaultOptions options)
    {
        if(string.IsNullOrWhiteSpace(options.Kv.EngineVersion))
        {
            throw new VaultConfigurationException("Vault engine version must not be empty.");
        }

        if (!EngineVersions.Contains(options.Kv.EngineVersion))
        {
            throw new VaultConfigurationException($"Invalid KV engine version: {options.Kv.EngineVersion}. Available versions are: V1 or V2).");
        }
    }

    private static async Task AddVaultAsync(this IConfigurationBuilder builder, VaultOptions options)
    {
        var (client, _) = GetClientAndSettings(options);
        if (options.Kv.Enabled)
        {
            var kvPath = options.Kv.Path;
            var mountPoint = options.Kv.MountPoint;
            if (string.IsNullOrWhiteSpace(kvPath))
            {
                throw new VaultConfigurationException("KV path is missing.");
            }

            if (string.IsNullOrWhiteSpace(mountPoint))
            {
                throw new VaultConfigurationException("KV mount point is missing.");
            }

            Console.WriteLine($"Loading settings from Vault: '{options.Url}', KV path: '{mountPoint}/{kvPath}'...");
            var jsonSerializer = new SystemTextJsonSerializer();
            var keyValueSecrets = new KeyValueSecrets(client, new OptionsWrapper<VaultOptions>(options),
                jsonSerializer);
            var secret = await keyValueSecrets.GetAsync(kvPath);
            var parser = new JsonParser();
            var json = jsonSerializer.Serialize(secret);
            var data = parser.Parse(json);
            var source = new MemoryConfigurationSource { InitialData = data! };
            builder.Add(source);
        }

        if (options.Pki.Enabled)
        {
            Console.WriteLine("Initializing Vault PKI.");
            await SetPkiSecretsAsync(client, options);
        }

        if (options.Lease.Count == 0)
        {
            return;
        }

        var configuration = new Dictionary<string, string>();
        foreach (var (key, lease) in options.Lease)
        {
            if (!lease.Enabled || string.IsNullOrWhiteSpace(lease.Type))
            {
                continue;
            }

            Console.WriteLine($"Initializing Vault lease for: '{key}', type: '{lease.Type}'.");
            await InitLeaseAsync(key, client, lease, configuration);
        }

        if (configuration.Count > 0)
        {
            var source = new MemoryConfigurationSource { InitialData = configuration! };
            builder.Add(source);
        }
    }

    private static Task InitLeaseAsync(
        string key,
        IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
        => options.Type.ToLowerInvariant() switch
        {
            "activedirectory" => SetActiveDirectorySecretsAsync(key, client, options, configuration),
            "azure" => SetAzureSecretsAsync(key, client, options, configuration),
            "consul" => SetConsulSecretsAsync(key, client, options, configuration),
            "database" => SetDatabaseSecretsAsync(key, client, options, configuration),
            "rabbitmq" => SetRabbitMqSecretsAsync(key, client, options, configuration),
            _ => Task.CompletedTask
        };

    private static async Task SetActiveDirectorySecretsAsync(
        string key,
        IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.ActiveDirectory;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.ActiveDirectory.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["username"] = credentials.Data.Username,
            ["currentPassword"] = credentials.Data.CurrentPassword,
            ["lastPassword"] = credentials.Data.LastPassword
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetAzureSecretsAsync(
        string key,
        IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.Azure;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.Azure.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["clientId"] = credentials.Data.ClientId,
            ["clientSecret"] = credentials.Data.ClientSecret
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetConsulSecretsAsync(
        string key,
        IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.Consul;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.Consul.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["token"] = credentials.Data.Token
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetDatabaseSecretsAsync(
        string key,
        IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.Database;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.Database.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["username"] = credentials.Data.Username,
            ["password"] = credentials.Data.Password
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetPkiSecretsAsync(IVaultClient client, VaultOptions options)
    {
        var issuer = new CertificatesIssuer(client, new OptionsWrapper<VaultOptions>(options));
        var certificate = await issuer.IssueAsync();
        if (certificate is not null)
        {
            CertificatesStore.Set(options.Pki.RoleName, certificate);
        }
    }

    private static async Task SetRabbitMqSecretsAsync(
        string key,
        IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.RabbitMQ;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.RabbitMQ.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["username"] = credentials.Data.Username,
            ["password"] = credentials.Data.Password
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static void SetSecrets(
        string key,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration, string name,
        Func<(object, Dictionary<string, string>, string, int, bool)> lease)
    {
        var createdAt = DateTime.UtcNow;
        var (credentials, values, leaseId, duration, renewable) = lease();
        SetTemplates(key, options, configuration, values);
        var leaseData = new LeaseData(name, leaseId, duration, renewable, createdAt, credentials);
        LeaseService.Set(key, leaseData);
    }

    private static (IVaultClient client, VaultClientSettings settings) GetClientAndSettings(VaultOptions options)
    {
        var settings = new VaultClientSettings(options.Url, GetAuthMethod(options.Authentication));
        var client = new VaultClient(settings);

        return (client, settings);
    }

    private static void SetTemplates(
        string key,
        VaultOptions.LeaseOptions lease,
        IDictionary<string, string> configuration,
        IDictionary<string, string> values)
    {
        if (lease.Templates.Count == 0)
        {
            return;
        }

        foreach (var (property, template) in lease.Templates)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(template))
            {
                continue;
            }

            var templateValue = $"{template}";
            templateValue = values.Aggregate(templateValue,
                (current, value) => current.Replace($"{{{{{value.Key}}}}}", value.Value));
            configuration.Add($"{key}:{property}", templateValue);
        }
    }

    private static IAuthMethodInfo GetAuthMethod(VaultOptions.AuthenticationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Type))
        {
            throw new VaultConfigurationException("Vault authentication type is empty.");
        }

        return options.Type.ToLowerInvariant() switch
        {
            "token" => new TokenAuthMethodInfo(options.Token.Token),
            "userpass" => new UserPassAuthMethodInfo(options.UserPass.Username, options.UserPass.Password),
            _ => throw new VaultAuthTypeNotSupportedException($"Vault authentication type: '{options.Type}' " +
                                                              "is not supported.", options.Type)
        };
    }

    public static IHttpClientBuilder AddVaultCertificatesHandler(this IHttpClientBuilder builder, IConfiguration configuration)
    {
        var section = configuration.GetSection(DefaultSectionName);
        var options = section.BindOptions<VaultOptions>();
        if (!options.Enabled || !options.Pki.Enabled || !options.Pki.HttpHandler.Enabled)
        {
            return builder;
        }

        var certificateName = options.Pki.HttpHandler.Certificate;
        if (string.IsNullOrWhiteSpace(certificateName))
        {
            throw new VaultConfigurationException("PKI HTTP handler certificate name is empty.");
        }

        var certificate = CertificatesStore.Get(certificateName);
        return certificate is null
            ? throw new VaultConfigurationException($"PKI HTTP handler certificate: '{certificateName}' was not found.")
            : builder.ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate);
                return handler;
            });
    }
}