using EasyNetQ;
using EasyNetQ.Consumer;
using Jalpan.Contexts.Accessors;
using Jalpan.Messaging.RabbitMQ.Exceptions;
using Jalpan.Messaging.RabbitMQ.Internals;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Jalpan.Messaging.Clients;
using Jalpan.Messaging.Subscribers;
using Jalpan.Handlers;

namespace Jalpan.Messaging.RabbitMQ;

public static class Extensions
{
    private const string DefaultSectionName = "rabbitmq";
    private const string RegistryKey = "messaging.rabbitmq";
    public static IJalpanBuilder AddRabbitMq(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;
        
        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<RabbitMqOptions>();
        builder.Services.Configure<RabbitMqOptions>(section);
        
        if (!options.Enabled || !builder.TryRegister(RegistryKey))
        {
            return builder;
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
        
        builder.Services.AddSingleton(bus);
        builder.Services.AddSingleton<IMessageBrokerClient, RabbitMqBrokerClient>();
        builder.Services.AddSingleton<IMessageSubscriber, RabbitMqMessageSubscriber>();
        builder.Services.AddSingleton<Internals.IMessageHandler, Internals.MessageHandler>();
        builder.Services.AddSingleton<IMessageTypeRegistry>(messageTypeRegistry);
        builder.Services.AddSingleton<IContextAccessor>(contextAccessor);
        builder.Services.AddSingleton<IMessageContextAccessor>(messageContextAccessor);

        return builder;
    }
    
    public static IServiceCollection AddMessagingErrorHandlingDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(IEventHandler<>), typeof(MessagingErrorHandlingEventHandlerDecorator<>));
        return services;
    }
    
    public static IServiceCollection AddErrorToMessageHandlerDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ErrorToMessageCommandHandlerDecorator<,>));
        return services;
    }

    internal static string ToMessageKey(this string messageType) => messageType.Underscore();
}