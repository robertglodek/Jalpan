using System.Diagnostics;

namespace Jalpan.Contexts;

public sealed class Context(
    string activityId,
    string? userId = null,
    string? messageId = null,
    string? role = null,
    IDictionary<string, string[]>? claims = null) : IContext
{
    public string ActivityId { get; } = activityId;
    public string? UserId { get; } = userId;
    public string? MessageId { get; } = messageId;
    public string? Role { get; } = role;
    public IDictionary<string, string[]>? Claims { get; } = claims;

    public Context() : this(Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString())
    {
    }
}