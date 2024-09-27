namespace Jalpan.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class MessageAttribute(
    string? exchange = null,
    string? topic = null,
    string? queue = null,
    string? queueType = null,
    string? errorQueue = null,
    string? subscriptionId = null) : Attribute
{
    public string Exchange { get; } = exchange ?? string.Empty;
    public string Topic { get; } = topic ?? string.Empty;
    public string Queue { get; } = queue ?? string.Empty;
    public string QueueType { get; } = queueType ?? string.Empty;
    public string ErrorQueue { get; } = errorQueue ?? string.Empty;
    public string SubscriptionId { get; } = subscriptionId ?? string.Empty;
}