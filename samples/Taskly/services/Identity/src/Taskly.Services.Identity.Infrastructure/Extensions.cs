using Jalpan;
using Jalpan.Auth.Jwt;
using Jalpan.Contexts;
using Jalpan.Logging.Serilog;
using Jalpan.Messaging;
using Jalpan.Messaging.RabbitMQ;
using Jalpan.Messaging.RabbitMQ.Streams;
using Jalpan.Persistence.MongoDB;
using Jalpan.Security;
using Jalpan.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Taskly.Services.Identity.Application.Context;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Infrastructure.Auth;
using Taskly.Services.Identity.Infrastructure.Mongo;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;
using Taskly.Services.Identity.Infrastructure.Mongo.Initializers;
using Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

namespace Taskly.Services.Identity.Infrastructure;

public static class Extensions
{
    public static IJalpanBuilder AddInfrastructure(this IJalpanBuilder builder)
    {
        builder
            .AddLogger()
            .AddMongo()
            .AddContexts()
            .AddDataContexts<IdentityDataContext>()
            .AddMessaging()
            .AddRabbitMq()
            .AddRabbitMqStreams()
            .AddJwt()
            .AddDistributedAccessTokenValidator()
            .AddSecurity()
            .AddTransactionalDecorators()
            .AddMongoRepository<RefreshTokenDocument, Guid>("refreshTokens")
            .AddMongoRepository<UserDocument, Guid>("users")
            .Services
            .AddErrorToMessageHandlerDecorators()
            .AddTransient<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddSingleton<IPasswordService, PasswordService>()
            .AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IInitializer, MongoDbInitializer>()
            .AddTransient<IJwtProvider, JwtProvider>()
            .AddTransient<IPasswordService, PasswordService>()
            .AddHealthChecks().AddMongoCheck();

        return builder;
    }

    private static IHealthChecksBuilder AddMongoCheck(this IHealthChecksBuilder builder)
        => builder.AddCheck<MongoHealthCheck>(MongoHealthCheck.Name);
}