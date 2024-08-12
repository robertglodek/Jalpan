namespace Jalpan.Secretc.Vault;

public interface ILeaseService
{
    IReadOnlyDictionary<string, LeaseData> All { get; }
    LeaseData? Get(string key);
    void Set(string key, LeaseData data);
}