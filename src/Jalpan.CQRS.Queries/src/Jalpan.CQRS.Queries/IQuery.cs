namespace Jalpan.CQRS.Queries;

/// <summary>
/// Marker
/// </summary>
public interface IQuery
{
}

public interface IQuery<T> : IQuery
{
}
