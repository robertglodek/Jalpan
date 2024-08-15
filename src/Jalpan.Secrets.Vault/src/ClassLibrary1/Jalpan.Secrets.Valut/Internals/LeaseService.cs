using Jalpan.Secrets.Vault;
using System.Collections.Concurrent;

namespace Jalpan.Secrets.Internals.Vault;

internal sealed class LeaseService : ILeaseService
{
    private static readonly ConcurrentDictionary<string, LeaseData> Secrets =
        new();

    public IReadOnlyDictionary<string, LeaseData> All => Secrets;

    public LeaseData? Get(string key) => Secrets.TryGetValue(key, out var data) ? data : null;

    public void Set(string key, LeaseData data)
    {
        Secrets.TryRemove(key, out _);
        Secrets.TryAdd(key, data);
    }
}