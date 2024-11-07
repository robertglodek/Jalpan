using System.Diagnostics;

namespace Jalpan.Contexts;

public sealed class Context(string activityId, string? userId = null, string? messageId = null)
    : IContext
{
    public string ActivityId { get; } = activityId;
    public string? UserId { get; } = userId;
    public string? MessageId { get; } = messageId;

    public Context() : this(Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString())
    {
    }
}