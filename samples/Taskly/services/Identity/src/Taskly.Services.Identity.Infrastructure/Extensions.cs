using Microsoft.Extensions.DependencyInjection;
using Taskly.Services.Identity.Infrastructure.Mongo;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

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
        .Services
            .AddTransient<IInitializer, MongoDbInitializer>();

        return builder;
    }
}
