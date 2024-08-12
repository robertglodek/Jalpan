namespace Jalpan.Types;

/// <summary>
/// Marker interface
/// </summary>
public interface ICommand
{
}

public interface ICommand<T> : ICommand
{
}