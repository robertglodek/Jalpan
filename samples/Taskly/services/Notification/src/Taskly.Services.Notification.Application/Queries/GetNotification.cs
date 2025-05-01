using Jalpan.Types;
using Taskly.Services.Notification.Application.DTO;

namespace Taskly.Services.Notification.Application.Queries;

public sealed class GetNotification(Guid id) : IQuery<NotificationDetailsDto>
{
    public Guid Id { get; } = id;
}
