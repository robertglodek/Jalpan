using Jalpan.Persistence.SqlServer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Persistence.SqlServer;

public static class Extensions
{
    private const string DefaultSectionName = "sqlserver";
    private const string RegistryKey = "persistence.sqlserver";

    public static IJalpanBuilder AddSqlServer<T>(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
        where T : DbContext
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<SqlServerOptions>();
        builder.Services.Configure<SqlServerOptions>(section);

        if (!builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        builder.Services.AddDbContext<T>(x => x.UseSqlServer(options.ConnectionString));
        builder.Services.AddHostedService<DatabaseInitializer<T>>();
        builder.Services.AddScoped<IUnitOfWork, SqlServerUnitOfWork>();
       
        return builder;
    }

    public static IJalpanBuilder AddSqlServerRepository<TEntity, TIdentifiable>(
        this IJalpanBuilder builder) where TEntity : class
    {
        builder.Services.AddTransient<ISqlServerRepository<TEntity, TIdentifiable>, SqlServerRepository<TEntity, TIdentifiable>>();

        return builder;
    }
}