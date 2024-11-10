using Jalpan.Messaging.Streams;
using Jalpan.Messaging.RabbitMQ.Streams.Publishers;
using Jalpan.Messaging.RabbitMQ.Streams.Subscribers;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.RabbitMQ.Streams;

public static class Extensions
{
    private const string SectionName = "rabbitmq:streams";
    private const string RegistryKey = "messaging.rabbitmq.streams";

    public static IJalpanBuilder AddRabbitMqStreams(this IJalpanBuilder builder)
    {
        var section = builder.Configuration.GetSection(SectionName);
        var options = section.BindOptions<RabbitMqStreamsOptions>();
        builder.Services.Configure<RabbitMqStreamsOptions>(section);

        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        builder.Services.AddSingleton<RabbitStreamManager>();
        builder.Services.AddHostedService<RabbitStreamInitializer>();
        builder.Services.AddSingleton<IStreamPublisher, RabbitMqStreamPublisher>();
        builder.Services.AddSingleton<IStreamSubscriber, RabbitMqStreamSubscriber>();

        return builder;
    }
}