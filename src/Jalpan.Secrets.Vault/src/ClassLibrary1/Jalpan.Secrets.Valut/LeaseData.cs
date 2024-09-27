namespace Jalpan.Secrets.Vault;

public class LeaseData(string type, string id, int duration, bool autoRenewal, DateTime createdAt, object data)
{
    public string Type { get; } = type;
    public string Id { get; } = id;
    public int Duration { get; } = duration;
    public bool AutoRenewal { get; } = autoRenewal;
    public DateTime CreatedAt { get; } = createdAt;
    public DateTime ExpiryAt { get; private set; } = createdAt.AddSeconds(duration);
    public object Data { get; } = data;

    public void Refresh(double duration)
    {
        ExpiryAt = ExpiryAt.AddSeconds(duration);
    }
}