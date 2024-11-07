using EasyNetQ;
using EasyNetQ.Consumer;
using Jalpan.Contexts.Accessors;
using Jalpan.Messaging.RabbitMQ.Exceptions;
using Jalpan.Messaging.RabbitMQ.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Jalpan.Messaging.Clients;
using Jalpan.Messaging.Subscribers;
using Jalpan.Handlers;

namespace Jalpan.Messaging.RabbitMQ;

public static class Extensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("rabbitmq");
        var options = section.BindOptions<RabbitMqOptions>();
        services.Configure<RabbitMqOptions>(section);
        
        if (!options.Enabled)
        {
            return services;
        }
        
        var contextAccessor = new ContextAccessor();
        var messageContextAccessor = new MessageContextAccessor();
        var messageTypeRegistry = new MessageTypeRegistry();
        
        var bus = RabbitHutch.CreateBus(options.ConnectionString,
            register =>
            {
                var systemTextJsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };
                register.EnableSystemTextJson(systemTextJsonOptions);
                register.Register(typeof(IConventions), typeof(CustomConventions));
                register.Register(typeof(IMessageSerializationStrategy), typeof(CustomMessageSerializationStrategy));
                register.Register(typeof(IHandlerCollectionFactory), typeof(CustomHandlerCollectionFactory));
                register.Register(typeof(IMessageTypeRegistry), messageTypeRegistry);
                register.Register(typeof(IContextAccessor), contextAccessor);
                register.Register(typeof(IMessageContextAccessor), messageContextAccessor);
            });
        
        services.AddSingleton(bus);
        services.AddSingleton<IMessageBrokerClient, RabbitMqBrokerClient>();
        services.AddSingleton<IMessageSubscriber, RabbitMqMessageSubscriber>();
        services.AddSingleton<Internals.IMessageHandler, Internals.MessageHandler>();
        services.AddSingleton<IMessageTypeRegistry>(messageTypeRegistry);
        services.AddSingleton<IContextAccessor>(contextAccessor);
        services.AddSingleton<IMessageContextAccessor>(messageContextAccessor);

        return services;
    }
    
    public static IServiceCollection AddMessagingErrorHandlingDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(IEventHandler<>), typeof(MessagingErrorHandlingEventHandlerDecorator<>));
        return services;
    }

    internal static string ToMessageKey(this string messageType) => messageType.Underscore();
}