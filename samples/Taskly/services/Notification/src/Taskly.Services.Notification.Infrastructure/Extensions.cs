using Jalpan;
using Jalpan.Auth.Jwt;
using Jalpan.Contexts;
using Jalpan.Discovery.Consul;
using Jalpan.Jobs.Quartz;
using Jalpan.LoadBalancing.Fabio;
using Jalpan.Logging.Serilog;
using Jalpan.Messaging.RabbitMQ;
using Jalpan.Metrics.OpenTelemetry;
using Jalpan.Persistence.MongoDB;
using Jalpan.Persistence.Redis;
using Jalpan.Tracing.OpenTelemetry;
using Jalpan.WebApi;
using Jalpan.WebApi.Contracts;
using Jalpan.WebApi.CORS;
using Jalpan.WebApi.Exceptions;
using Jalpan.WebApi.Networking;
using Jalpan.WebApi.Swagger;
using Jalpan.WebApi.Swagger.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Taskly.Services.Identity.Infrastructure.Exceptions.Mappers;
using Taskly.Services.Notification.Domain.Repositories;
using Taskly.Services.Notification.Infrastructure.Mongo.Documents;
using Taskly.Services.Notification.Infrastructure.Mongo.Repositories;

namespace Taskly.Services.Notification.Infrastructure;

public static class Extensions
{
    public static IJalpanBuilder AddInfrastructure(this IJalpanBuilder builder)
    {
        builder
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddContexts()
            .AddCorsPolicy()
            .AddSwaggerDocs(swaggerGenOptions: options => options.SchemaFilter<EnumSchemaFilter>())
            .AddHeadersForwarding()
            .AddConsul()
            .AddFabio()
            .AddLogger()
            .AddTracing()
            .AddMetrics()
            .AddMongo()
            .AddRedis()
            .AddDistributedAccessTokenValidator()
            .AddMongoRepository<UserDocument, Guid>("users")
            .AddMongoRepository<NotificationDocument, Guid>("notifications")
            .AddQuartz()
            .Services
            .AddMemoryCache()
            .AddHttpContextAccessor()
            .AddErrorToMessageHandlerDecorators()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<INotificationRepository, NotificationRepository>()
            .AddHealthChecks().AddMongoCheck().AddSelfCheck().AddRedisCheck();

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseErrorHandler();
        app.UseHeadersForwarding();
        app.UseCorsPolicy();
        app.UseSwaggerDocs();
        app.UseAuthentication();
        app.UseRouting();
        app.UsePublicContracts();
        app.UseMetrics();
        app.UseContextLogger();
        app.UseAccessTokenValidator();
        app.UseSerilogRequestLogging();

        return app;
    }
}
