using Jalpan;
using Jalpan.Types;

namespace Taskly.Services.Task.Application.Commands;

public sealed class DeleteTask(Guid id) : ICommand<Empty>
{
    public Guid Id { get; } = id;
}
