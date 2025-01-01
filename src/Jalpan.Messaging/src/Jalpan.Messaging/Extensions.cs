using Jalpan.Messaging.Brokers;
using Jalpan.Messaging.Clients;
using Jalpan.Messaging.Streams;
using Jalpan.Messaging.Streams.Serialization;
using Jalpan.Messaging.Subscribers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Jalpan.Messaging.Exceptions;

namespace Jalpan.Messaging;

public static class Extensions
{
    private const string DefaultMessagingSectionName = "messaging";

    public static IJalpanBuilder AddMessaging(this IJalpanBuilder builder, string sectionName = DefaultMessagingSectionName)
    {
        var section = builder.Configuration.GetSection(sectionName);
        builder.Services.Configure<AppOptions>(section);

        builder.Services.AddTransient<IMessageBroker, MessageBroker>();
        builder.Services.AddSingleton<IMessageBrokerClient, DefaultMessageBrokerClient>();
        builder.Services.AddSingleton<IMessageSubscriber, DefaultMessageSubscriber>();
        builder.Services.AddSingleton<IMessagingExceptionPolicyResolver, DefaultMessagingExceptionPolicyResolver>();
        builder.Services.AddSingleton<IExceptionToMessageResolver, DefaultExceptionToMessageResolver>();
        builder.Services.AddSingleton<IMessagingExceptionPolicyHandler, DefaultMessagingExceptionPolicyHandler>();
        builder.Services.AddSingleton<IStreamSerializer, SystemTextJsonStreamSerializer>();
        builder.Services.AddSingleton<IStreamPublisher, DefaultStreamPublisher>();
        builder.Services.AddSingleton<IStreamSubscriber, DefaultStreamSubscriber>();
        
        return builder;
    }
    
    public static IJalpanBuilder AddExceptionToMessageResolver<T>(this IJalpanBuilder builder) where T : class, IExceptionToMessageResolver
    {
        builder.Services.AddSingleton<IExceptionToMessageResolver, T>();
        return builder;
    }
    
    public static IMessageSubscriber Subscribe(this IApplicationBuilder app)
        => app.ApplicationServices.GetRequiredService<IMessageSubscriber>();
}