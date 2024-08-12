namespace Jalpan;

public readonly struct Empty : IEquatable<Empty>, IComparable<Empty>, IComparable
{
    public static Empty Value => default;
    public static Task<Empty> Task { get; } = System.Threading.Tasks.Task.FromResult(Value);
    public int CompareTo(Empty other) => 0;
    public int CompareTo(object? obj) => 0;
    public override int GetHashCode() => 0;
    public bool Equals(Empty other) => true;
    public override bool Equals(object? obj) => obj is Empty;
    public static bool operator ==(Empty _, Empty _2) => true;
    public static bool operator !=(Empty _, Empty _2) => false;
    public override string ToString() => string.Empty;
    public static bool operator <(Empty left, Empty right) => left.CompareTo(right) < 0;
    public static bool operator <=(Empty left, Empty right) => left.CompareTo(right) <= 0;
    public static bool operator >(Empty left, Empty right) => left.CompareTo(right) > 0;
    public static bool operator >=(Empty left, Empty right) => left.CompareTo(right) >= 0;
}
