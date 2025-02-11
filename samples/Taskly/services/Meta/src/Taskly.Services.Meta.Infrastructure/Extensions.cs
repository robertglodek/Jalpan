using Jalpan;
using Jalpan.Auth.Jwt;
using Jalpan.Contexts;
using Jalpan.Discovery.Consul;
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
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Infrastructure.Exceptions.Mappers;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;
using Taskly.Services.Meta.Infrastructure.Mongo.Initializers;
using Taskly.Services.Meta.Infrastructure.Mongo.Repositories;

namespace Taskly.Services.Meta.Infrastructure;

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
            .AddMongoRepository<GoalDocument, Guid>("goals")
            .AddMongoRepository<SectionDocument, Guid>("sections")
            .AddMongoRepository<TagDocument, Guid>("tags")
            .Services
            .AddMemoryCache()
            .AddHttpContextAccessor()
            .AddErrorToMessageHandlerDecorators()
            .AddTransient<IGoalRepository, GoalRepository>()
            .AddTransient<ISectionRepository, SectionRepository>()
            .AddTransient<ITagRepository, TagRepository>()
            .AddTransient<IInitializer, MongoDbInitializer>()
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
        app.UseAuthorization();
        app.UseContextLogger();
        app.UseAccessTokenValidator();
        app.UseSerilogRequestLogging();

        return app;
    }
}