using Jalpan.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.Idempotency.Inbox;

public static class Extensions
{
    private const string DefaultSectionName = "inbox";
    private const string RegistryKey = "messaging.inbox";

    public static IJalpanBuilder AddInbox(this IJalpanBuilder builder, string sectionName = DefaultSectionName,
        Action<IMessageInboxConfigurator>? configure = null)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<InboxOptions>();
        builder.Services.Configure<InboxOptions>(section);

        if (!builder.TryRegister(RegistryKey) && !options.Enabled)
        {
            return builder;
        }

        builder.Services.AddHostedService<InboxCleaner>();
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(InboxEventHandlerDecorator<>));
        
        var configurator = new MessageInboxConfigurator(builder, options);
        configure?.Invoke(configurator);

        return builder;
    }
}