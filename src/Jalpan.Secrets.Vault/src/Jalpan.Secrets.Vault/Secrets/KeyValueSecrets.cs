using Jalpan.Secrets.Vault.Exceptions;
using Jalpan.Serialization;
using Microsoft.Extensions.Options;
using VaultSharp;

namespace Jalpan.Secrets.Vault.Secrets;

internal sealed class KeyValueSecrets(IVaultClient client, IOptions<VaultOptions> options, IJsonSerializer jsonSerializer) : IKeyValueSecrets
{
    private readonly VaultOptions _options = options.Value;

    public async Task<T> GetAsync<T>(string path)
        => jsonSerializer.Deserialize<T>(jsonSerializer.Serialize(await GetAsync(path)))!;

    public async Task<IDictionary<string, object>> GetAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new VaultException("Vault KV secret path can not be empty.");
        }

        try
        {
            switch (_options.Kv.EngineVersion)
            {
                case "V1":
                    var secretV1 = await client.V1.Secrets.KeyValue.V1.ReadSecretAsync(path,
                        _options.Kv.MountPoint);
                    return secretV1.Data;
                case "V2":
                    var secretV2 = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path,
                        _options.Kv.Version, _options.Kv.MountPoint);
                    return secretV2.Data.Data;
                default:
                    throw new VaultException($"Invalid KV engine version: {_options.Kv.EngineVersion}.");
            }
        }
        catch (Exception exception)
        {
            throw new VaultException($"Getting Vault secret for path: '{path}' caused an error. " +
                                     $"{exception.Message}", exception);
        }
    }
}