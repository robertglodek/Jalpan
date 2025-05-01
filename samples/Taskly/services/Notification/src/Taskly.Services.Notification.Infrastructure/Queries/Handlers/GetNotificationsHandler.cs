using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Notification.Application.DTO;
using Taskly.Services.Notification.Application.Queries;
using Taskly.Services.Notification.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Notification.Infrastructure.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetNotificationsHandler(IContextProvider contextProvider, IMongoDbRepository<NotificationDocument, Guid> notificationRepository)
    : IQueryHandler<GetNotifications, IEnumerable<NotificationDto>>
{
    public async Task<IEnumerable<NotificationDto>?> HandleAsync(GetNotifications query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var notifications = await notificationRepository.FindAsync(n => n.UserId == Guid.Parse(context.UserId!), cancellationToken);

        return notifications.Select(n => n.AsDto());
    }
}
