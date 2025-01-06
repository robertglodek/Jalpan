using Jalpan.Messaging.Idempotency.Inbox;
using Jalpan.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.Idempotency.MongoDB.Inbox;

public static class Extensions
{
    private const string DefaultSectionName = "inbox:mongo";
    private const string RegistryKey = "messaging.inbox.mongo";
    internal const string DefaultInboxCollectionName = "inbox";

    public static IMessageInboxConfigurator AddMongo(this IMessageInboxConfigurator configurator, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = configurator.Builder.Configuration.GetSection(sectionName);
        var mongoInboxOptions = section.BindOptions<MongoDbInboxOptions>();
        configurator.Builder.Services.Configure<MongoDbInboxOptions>(section);

        if (!configurator.Builder.TryRegister(RegistryKey))
        {
            return configurator;
        }

        var collection = string.IsNullOrWhiteSpace(mongoInboxOptions.Collection) ? DefaultInboxCollectionName : mongoInboxOptions.Collection;

        configurator.Builder.AddMongoRepository<InboxMessage, string>(collection);
        configurator.Builder.Services.AddTransient<IInitializer, MongoDbInboxInitializer>();
        configurator.Builder.Services.AddTransient<IInbox, MongoDbInbox>();

        return configurator;
    }
}
