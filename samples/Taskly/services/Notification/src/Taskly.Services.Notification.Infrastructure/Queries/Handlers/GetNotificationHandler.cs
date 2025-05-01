using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Notification.Application.DTO;
using Taskly.Services.Notification.Application.Queries;
using Taskly.Services.Notification.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Notification.Infrastructure.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetNotificationHandler(IContextProvider contextProvider, IMongoDbRepository<NotificationDocument, Guid> notificationRepository)
    : IQueryHandler<GetNotification, NotificationDetailsDto>
{  
    public async Task<NotificationDetailsDto?> HandleAsync(GetNotification query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var notification = await notificationRepository.GetAsync(n => n.UserId == Guid.Parse(context.UserId!) && n.Id == query.Id, cancellationToken);

        return notification?.AsDetailsDto();
    }
}
