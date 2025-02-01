using Jalpan;
using Jalpan.Contexts;
using Jalpan.Logging.Serilog;
using Jalpan.Metrics.OpenTelemetry;
using Jalpan.WebApi.Contracts;
using Jalpan.WebApi.CORS;
using Jalpan.WebApi.Exceptions;
using Jalpan.WebApi.Networking;
using Jalpan.WebApi.Swagger;
using Jalpan.WebApi.Swagger.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Serilog;
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
            .AddMessaging()
            .AddRabbitMq()
            .AddRabbitMqStreams()
            .AddConsul()
            .AddFabio()
            .AddSecurity()
            .AddLogger()
            .AddTracing()
            .AddMetrics()
            .AddMongo()
            .AddRedis()
            .AddDistributedAccessTokenValidator()
            .AddMongoRepository<TagDocument, Guid>("refreshTokens")
            .AddMongoRepository<GoalDocument, Guid>("users")
            .Services
            .AddMemoryCache()
            .AddHttpContextAccessor()
            .AddErrorToMessageHandlerDecorators()
            .AddDataContextDecorators()
            .AddTransient<IRefreshTokenRepository, SectionRepository>()
            .AddSingleton<IPasswordService, PasswordService>()
            .AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>()
            .AddTransient<IUserRepository, TagRepository>()
            .AddTransient<IInitializer, MongoDbInitializer>()
            .AddTransient<IJwtProvider, JwtProvider>()
            .AddTransient<IPasswordService, PasswordService>()
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