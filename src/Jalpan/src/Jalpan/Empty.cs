namespace Jalpan;

public readonly struct Empty : IEquatable<Empty>
{
    public static Empty Value => default;

    public static Task<Empty> Task { get; } = System.Threading.Tasks.Task.FromResult(Value);

    public override int GetHashCode() => 0;

    public bool Equals(Empty other) => true;

    public override bool Equals(object? obj) => obj is Empty;

    public override string ToString() => string.Empty;

    public static bool operator ==(Empty _, Empty _2) => true;

    public static bool operator !=(Empty _, Empty _2) => false;

    public static async Task<Empty> ExecuteAsync(Func<Task> action)

    {
        ArgumentNullException.ThrowIfNull(action);
        await action();
        return Value;
    }

    public static Empty Execute(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action();
        return Value;
    }
}