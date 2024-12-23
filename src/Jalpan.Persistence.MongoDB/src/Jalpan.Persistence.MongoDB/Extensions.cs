﻿using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Jalpan.Persistence.MongoDB.Repositories;
using Jalpan.Types;

namespace Jalpan.Persistence.MongoDB;

public static class Extensions
{
    private const string DefaultSectionName = "mongo";
    private const string RegistryKey = "persistence.mongoDb";

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

        builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(options.ConnectionString));
        builder.Services.AddTransient(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.Database);
        });

        builder.Services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();

        return builder;
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
}
