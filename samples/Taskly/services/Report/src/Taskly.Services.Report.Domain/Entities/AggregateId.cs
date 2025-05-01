using Taskly.Services.Report.Domain.Exceptions;

namespace Taskly.Services.Report.Domain.Entities;

public sealed class AggregateId : IEquatable<AggregateId>
{
    public Guid Value { get; }

    public AggregateId()
    {
        Value = Guid.NewGuid();
    }

    public AggregateId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidAggregateIdException();
        }

        Value = value;
    }

    public bool Equals(AggregateId? other)
    {
        if (other is null) return false;
        return ReferenceEquals(this, other) || Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((AggregateId) obj);
    }

    public override string ToString() => Value.ToString();
    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator Guid(AggregateId id) => id.Value;

    public static implicit operator AggregateId(Guid id) => new(id);
}