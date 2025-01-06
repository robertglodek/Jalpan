using Jalpan.Messaging.Idempotency.Outbox;
using Jalpan.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.Idempotency.MongoDB.Outbox;

public static class Extensions
{
    private const string DefaultSectionName = "outbox:mongo";
    private const string RegistryKey = "messaging.outbox.mongo";
    internal const string DefaultOutboxCollectionName = "outbox";

    public static IMessageOutboxConfigurator AddMongo(
        this IMessageOutboxConfigurator configurator,
        string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = configurator.Builder.Configuration.GetSection(sectionName);
        var mongoOutboxOptions = section.BindOptions<MongoDbOutboxOptions>();
        configurator.Builder.Services.Configure<MongoDbOutboxOptions>(section);

        if (!configurator.Builder.TryRegister(RegistryKey))
        {
            return configurator;
        }

        var collection = string.IsNullOrWhiteSpace(mongoOutboxOptions.Collection)
            ? DefaultOutboxCollectionName : mongoOutboxOptions.Collection;

        configurator.Builder.AddMongoRepository<OutboxMessage, string>(collection);
        configurator.Builder.Services.AddTransient<IOutbox, MongoDbOutbox>();
        configurator.Builder.Services.AddTransient<IInitializer, MongoDbOutboxInitializer>();

        return configurator;
    }
}
