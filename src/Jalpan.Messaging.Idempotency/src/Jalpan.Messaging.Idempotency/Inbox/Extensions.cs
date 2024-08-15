using Jalpan.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.Idempotency.Inbox;

public static class Extensions
{
    private const string SectionName = "inbox";
    private const string RegistryName = "messaging.inbox";

    public static IJalpanBuilder AddInbox(this IJalpanBuilder builder, string sectionName = SectionName)
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
        var options = section.BindOptions<InboxOptions>();
        builder.Services.Configure<InboxOptions>(section);

        if (!options.Enabled)
        {
            return builder;
        }

        builder.Services.AddHostedService<InboxCleaner>();
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(InboxEventHandlerDecorator<>));

        return builder;
    }
}