using Jalpan.Handlers;
using Jalpan.Messaging.Brokers;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.Idempotency.Outbox;

public static class Extensions
{
    private const string SectionName = "outbox";
    private const string RegistryName = "messaging.outbox";

    public static IJalpanBuilder AddOutbox(this IJalpanBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<OutboxOptions>();
        builder.Services.Configure<OutboxOptions>(section);
        
        if (!options.Enabled)
        {
            return builder;
        }

        builder.Services.AddTransient<IMessageBroker, OutboxMessageBroker>();
        builder.Services.AddHostedService<OutboxSender>();
        builder.Services.AddHostedService<OutboxCleaner>();

        return builder;
    }

    public static IServiceCollection AddOutboxInstantSenderDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxInstantSenderEventHandlerDecorator<>));

        return services;
    }
}