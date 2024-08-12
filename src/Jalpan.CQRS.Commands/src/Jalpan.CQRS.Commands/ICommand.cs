using Jalpan.Types;

namespace Jalpan.CQRS.Commands;

/// <summary>
/// Marker
/// </summary>
public interface ICommand: IMessage
{
}

public interface ICommand<TResponse>: ICommand
{
}
