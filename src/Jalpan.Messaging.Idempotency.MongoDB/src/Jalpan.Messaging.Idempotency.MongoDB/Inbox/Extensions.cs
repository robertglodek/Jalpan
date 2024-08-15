using Jalpan.Messaging.Idempotency.Inbox;
using Jalpan.Persistance.MongoDB;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.Idempotency.MongoDB.Inbox;

public static class Extensions
{
    private const string SectionName = "inbox:mongo";
    private const string RegistryName = "messaging.inbox.mongo";

    public static IMessageInboxConfigurator AddMongo(this IMessageInboxConfigurator configurator, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        if (!configurator.Builder.TryRegister(RegistryName))
        {
            return configurator;
        }

        var section = configurator.Builder.Configuration.GetSection(sectionName);
        var mongoInboxOptions = section.BindOptions<MongoDbInboxOptions>();
        configurator.Builder.Services.Configure<MongoDbInboxOptions>(section);

        var collection = string.IsNullOrWhiteSpace(mongoInboxOptions.Collection)
            ? "inbox" 
            : mongoInboxOptions.Collection;

        configurator.Builder.AddMongoRepository<InboxMessage, string>(collection);
        configurator.Builder.AddInitializer<MongoDbInboxInitializer>();
        configurator.Builder.Services.AddTransient<IInbox, MongoDbInbox>();
        configurator.Builder.Services.AddTransient<MongoDbInboxInitializer>();

        return configurator;
    }
}
