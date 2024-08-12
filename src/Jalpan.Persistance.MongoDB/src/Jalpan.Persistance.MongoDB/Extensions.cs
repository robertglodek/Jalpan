using Jalpan.Persistance.MongoDB.Factories;
using Jalpan.Persistance.MongoDB.Initializers;
using Jalpan.Persistance.MongoDB.Seeders;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Jalpan.Persistance.MongoDB.Repositories;
using Jalpan.Types;

namespace Jalpan.Persistance.MongoDB;

public static class Extensions
{
    private const string SectionName = "mongo";
    private const string RegistryName = "persistence.mongoDb";

    public static IJalpanBuilder AddMongo(
        this IJalpanBuilder builder,
        string sectionName = SectionName,
        Type? seederType = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<MongoDbOptions>();
        builder.Services.Configure<MongoDbOptions>(section);

        return builder.AddMongo(options, seederType);
    }

    public static IJalpanBuilder AddMongo(
        this IJalpanBuilder builder,
        MongoDbOptions mongoOptions,
        Type? seederType = null)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoOptions.ConnectionString));
        builder.Services.AddTransient(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoOptions.Database);
        });
        builder.Services.AddTransient<IMongoDbInitializer, MongoDbInitializer>();
        builder.Services.AddTransient<IMongoSessionFactory, MongoSessionFactory>();

        if (seederType is null)
        {
            builder.Services.AddTransient<IMongoDbSeeder, MongoDbSeeder>();
        }
        else
        {
            builder.Services.AddTransient(typeof(IMongoDbSeeder), seederType);
        }

        builder.AddInitializer<IMongoDbInitializer>();
      
        return builder;
    }

    public static IJalpanBuilder AddMongoRepository<TEntity, TIdentifiable>(
        this IJalpanBuilder builder,
        string collectionName)
        where TEntity : IIdentifiable<TIdentifiable>
    {
        builder.Services.AddTransient<IMongoRepository<TEntity, TIdentifiable>>(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return new MongoRepository<TEntity, TIdentifiable>(database, collectionName);
        });

        return builder;
    }
}
