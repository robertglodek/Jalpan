using Jalpan.Messaging.Streams;
using Jalpan.Messaging.RabbitMQ.Streams.Publishers;
using Jalpan.Messaging.RabbitMQ.Streams.Subscribers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Messaging.RabbitMQ.Streams;

public static class Extensions
{
    private const string SectionName = "rabbitmq:streams";

    public static IServiceCollection AddRabbitMqStreams(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.BindOptions<RabbitMQStreamsOptions>();
        services.Configure<RabbitMQStreamsOptions>(section);

        if (!options.Enabled)
        {
            return services;
        }

        services.AddSingleton<RabbitStreamManager>();
        services.AddHostedService<RabbitStreamInitializer>();
        services.AddSingleton<IStreamPublisher, RabbitMqStreamPublisher>();
        services.AddSingleton<IStreamSubscriber, RabbitMqStreamSubscriber>();

        return services;
    }
}