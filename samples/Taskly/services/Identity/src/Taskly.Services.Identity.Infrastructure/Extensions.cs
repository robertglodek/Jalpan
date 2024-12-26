using Jalpan;
using Jalpan.Auth.Jwt;
using Jalpan.Contexts;
using Jalpan.Logging.Serilog;
using Jalpan.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Infrastructure.Auth;
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
            .AddJwt()
            .AddMongoRepository<RefreshTokenDocument, Guid>("refreshTokens")
            .AddMongoRepository<UserDocument, Guid>("users")
            .Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IInitializer, MongoDbInitializer>()
            .AddTransient<IJwtProvider, JwtProvider>()
            .AddTransient<IPasswordService, PasswordService>();

        return builder;
    }
}