using MongoDB.Driver;

namespace Jalpan.Persistence.MongoDB;

public static class MongoDbSessionContext
{
    private static readonly AsyncLocal<IClientSessionHandle?> _currentSession = new();
    public static IClientSessionHandle? CurrentSession
    {
        get => _currentSession.Value;
        set => _currentSession.Value = value;
    }
}