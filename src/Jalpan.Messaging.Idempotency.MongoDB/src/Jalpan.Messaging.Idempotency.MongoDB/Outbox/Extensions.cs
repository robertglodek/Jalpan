using Jalpan.Messaging.Idempotency.Outbox;
using Jalpan.Persistance.MongoDB;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.Idempotency.MongoDB.Outbox;

public static class Extensions
{
    private const string SectionName = "outbox:mongo";
    private const string RegistryName = "messaging.outbox.mongo";

    public static IMessageOutboxConfigurator AddMongo(this IMessageOutboxConfigurator configurator, string sectionName = SectionName)
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
        var mongoOutboxOptions = section.BindOptions<MongoDbOutboxOptions>();
        configurator.Builder.Services.Configure<MongoDbOutboxOptions>(section);

        var collection = string.IsNullOrWhiteSpace(mongoOutboxOptions.Collection)
            ? "Outbox"
            : mongoOutboxOptions.Collection;

        configurator.Builder.AddMongoRepository<OutboxMessage, string>(collection);
        configurator.Builder.AddInitializer<MongoDbOutboxInitializer>();
        configurator.Builder.Services.AddTransient<IOutbox, MongoDbOutbox>();
        configurator.Builder.Services.AddTransient<MongoDbOutboxInitializer>();

        return configurator;
    }
}
