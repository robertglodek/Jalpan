using Jalpan;
using Jalpan.Types;

namespace Taskly.Services.Notification.Application.Commands;

public sealed class DeleteNotification(Guid id) : ICommand<Empty>
{
    public Guid Id { get; } = id;
}
