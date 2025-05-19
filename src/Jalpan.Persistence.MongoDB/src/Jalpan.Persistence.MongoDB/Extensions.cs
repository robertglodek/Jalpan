using System.Linq.Expressions;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Jalpan.Persistence.MongoDB.Repositories;
using Jalpan.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace Jalpan.Persistence.MongoDB;

public static class Extensions
{
    private const string DefaultSectionName = "mongo";
    private const string RegistryKey = "persistence.mongoDb";
    private const string DefaultMongoHealthCheckName = "mongo_health_check";

    public static IJalpanBuilder AddMongo(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<MongoDbOptions>();
        builder.Services.Configure<MongoDbOptions>(section);

        if (!builder.TryRegister(RegistryKey))
        {
            return builder;
        }
        
        ConfigureConventions();

        builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(options.ConnectionString));
        builder.Services.AddTransient(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.Database);
        });

        builder.Services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();

        return builder;
    }

    public static async Task CreateIndexAsync<TModel>(
        this IMongoCollection<TModel> collection,
        bool isUnique,
        params Expression<Func<TModel, object>>[] fields)
    {
        var keys = Builders<TModel>.IndexKeys.Ascending(fields[0]);

        for (var i = 1; i < fields.Length; i++)
            keys = keys.Ascending(fields[i]);

        var options = new CreateIndexOptions<TModel> { Unique = isUnique };
        var createIndexModel = new CreateIndexModel<TModel>(keys, options);
        await collection.Indexes.CreateOneAsync(createIndexModel);
    }

    public static async Task DropIndexAsync<TModel>(
        this IMongoCollection<TModel> collection,
        params Expression<Func<TModel, object>>[] fields)
    {
        if (fields == null || fields.Length == 0)
            throw new ArgumentException("At least one field must be specified.", nameof(fields));

        // Build index keys
        var keys = Builders<TModel>.IndexKeys.Ascending(fields[0]);
        for (var i = 1; i < fields.Length; i++)
            keys = keys.Ascending(fields[i]);

        // Use the new RenderArgs overload
        var args = new RenderArgs<TModel>(
            collection.DocumentSerializer,
            collection.Settings.SerializerRegistry
        );

        var renderedKeys = keys.Render(args);

        // MongoDB default naming convention: field1_1_field2_1_...
        var indexName = string.Join("_", renderedKeys.Names.Select(name => $"{name}_1"));

        await collection.Indexes.DropOneAsync(indexName);
    }

    public static IJalpanBuilder AddMongoRepository<TEntity, TIdentifiable>(
        this IJalpanBuilder builder,
        string collectionName)
        where TEntity : IIdentifiable<TIdentifiable>
    {
        builder.Services.AddTransient<IMongoDbRepository<TEntity, TIdentifiable>>(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return new MongoDbRepository<TEntity, TIdentifiable>(database, collectionName);
        });

        return builder;
    }

    private static void ConfigureConventions()
    {
        var conventionPack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String)
        };
        
        ConventionRegistry.Register("EnumStringConvention", conventionPack, _ => true);
    }

    public static IHealthChecksBuilder AddMongoCheck(this IHealthChecksBuilder builder,
        string name = DefaultMongoHealthCheckName)
        => builder.AddCheck<MongoDbHealthCheck>(name);
}