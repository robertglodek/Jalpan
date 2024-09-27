using System.Diagnostics;

namespace Jalpan.Contexts;

public sealed class Context : IContext
{
    public string ActivityId { get; }
    public string? UserId { get; }
    public string? MessageId { get; }

    public Context()
    {
        ActivityId = Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString();
    }

    public Context(string activityId, string? userId = null, string? messageId = null)
    {
        ActivityId = activityId;
        UserId = userId;
        MessageId = messageId;
    }
}