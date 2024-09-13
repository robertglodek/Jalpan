using Jalpan.Persistance.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Persistance.Postgres;

public static class Extensions
{
    private const string SectionName = "postgres";
    private const string RegistryName = "persistence.postgres";

    public static IJalpanBuilder AddPostgres<T>(this IJalpanBuilder builder, string sectionName = SectionName)
        where T : DbContext
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<PostgresOptions>();
        builder.Services.Configure<PostgresOptions>(section);

        if (!builder.TryRegister(RegistryName))
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