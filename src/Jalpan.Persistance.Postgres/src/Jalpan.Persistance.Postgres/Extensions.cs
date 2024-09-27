using Jalpan.Persistance.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Persistance.Postgres;

public static class Extensions
{
    private const string DefaultSectionName = "postgres";
    private const string RegistryKey = "persistence.postgres";

    public static IJalpanBuilder AddPostgres<T>(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
        where T : DbContext
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<PostgresOptions>();
        builder.Services.Configure<PostgresOptions>(section);

        if (!builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        builder.Services.AddDbContext<T>(x => x.UseNpgsql(options.ConnectionString));
        builder.Services.AddHostedService<DatabaseInitializer<T>>();
        builder.Services.AddScoped<IUnitOfWork, PostgresUnitOfWork>();
       
        return builder;
    }

    public static IJalpanBuilder AddPostgresRepository<TEntity, TIdentifiable>(
        this IJalpanBuilder builder) where TEntity : class
    {
        builder.Services.AddTransient<IPostgresRepository<TEntity, TIdentifiable>, PostgresRepository<TEntity, TIdentifiable>>();

        return builder;
    }
}