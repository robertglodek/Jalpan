﻿using Jalpan;
using Jalpan.Auth.Jwt;
using Jalpan.Contexts;
using Jalpan.Discovery.Consul;
using Jalpan.LoadBalancing.Fabio;
using Jalpan.Logging.Serilog;
using Jalpan.Messaging;
using Jalpan.Messaging.RabbitMQ;
using Jalpan.Messaging.RabbitMQ.Streams;
using Jalpan.Metrics.OpenTelemetry;
using Jalpan.Persistence.MongoDB;
using Jalpan.Persistence.Redis;
using Jalpan.Security;
using Jalpan.Tracing.OpenTelemetry;
using Jalpan.WebApi;
using Jalpan.WebApi.Contracts;
using Jalpan.WebApi.CORS;
using Jalpan.WebApi.Exceptions;
using Jalpan.WebApi.Networking;
using Jalpan.WebApi.Swagger;
using Jalpan.WebApi.Swagger.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Infrastructure.Auth;
using Taskly.Services.Identity.Infrastructure.Exceptions.Mappers;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;
using Taskly.Services.Identity.Infrastructure.Mongo.Initializers;
using Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

namespace Taskly.Services.Identity.Infrastructure;

public static class Extensions
{
    public static IJalpanBuilder AddInfrastructure(this IJalpanBuilder builder)
    {
        builder
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddContexts()
            .AddJwt()
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
            .AddMongoRepository<RefreshTokenDocument, Guid>("refreshTokens")
            .AddMongoRepository<UserDocument, Guid>("users")
            .Services
            .AddMemoryCache()
            .AddHttpContextAccessor()
            .AddErrorToMessageHandlerDecorators()
            .AddTransient<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddSingleton<IPasswordService, PasswordService>()
            .AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>()
            .AddTransient<IUserRepository, UserRepository>()
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
        app.UseContextLogger();
        app.UseAccessTokenValidator();
        app.UseSerilogRequestLogging();

        return app;
    }
}