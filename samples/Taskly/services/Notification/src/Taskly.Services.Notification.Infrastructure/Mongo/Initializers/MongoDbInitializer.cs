using Jalpan;
using Jalpan.Persistence.MongoDB;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Notification.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Initializers;

[UsedImplicitly]
internal sealed class MongoDbInitializer(
    IMongoDbRepository<NotificationDocument, Guid> notificationRepository,
    IMongoDbRepository<UserDocument, Guid> userRepository) : IInitializer
{
    public async Task InitializeAsync()
    {
        await InitializeNotificationsAsync();
        await InitializeUsersAsync();
    }

    private async Task InitializeNotificationsAsync()
    {
        await notificationRepository.Collection.CreateIndexAsync(false, u => u.UserId);
    }

    private async Task InitializeUsersAsync()
    {
        await userRepository.Collection.CreateIndexAsync(true, u => u.Email);
    }
}