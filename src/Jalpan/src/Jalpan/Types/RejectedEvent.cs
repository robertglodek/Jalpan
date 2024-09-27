namespace Jalpan.Types;

public class RejectedEvent(string reason, string code) : IRejectedEvent
{
    public string Reason { get; } = reason;
    public string Code { get; } = code;

    public static IRejectedEvent For(string name)
        => new RejectedEvent($"There was an error when executing: " +
                             $"{name}", $"{name}_error");
}
