﻿using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Notification.Domain.Repositories;
using Taskly.Services.Notification.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Repositories;

internal class NotificationRepository(IMongoDbRepository<NotificationDocument, Guid> repository) : INotificationScheduleRepository
{
    public async Task AddAsync(Domain.Entities.NotificationSchedule notification, CancellationToken cancellationToken = default)
        => await repository.AddAsync(notification.AsDocument(), cancellationToken: cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken: cancellationToken);

    public async Task<Domain.Entities.NotificationSchedule?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => (await repository.GetAsync(id, cancellationToken: cancellationToken))?.AsEntity();

    public async Task UpdateAsync(Domain.Entities.NotificationSchedule notification, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(notification.AsDocument(), cancellationToken: cancellationToken);
}
