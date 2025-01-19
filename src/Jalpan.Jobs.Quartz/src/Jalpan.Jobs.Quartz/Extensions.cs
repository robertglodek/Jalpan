using Jalpan.Jobs.Quartz.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Jalpan.Jobs.Quartz;

public static class Extensions
{
    private const string DefaultSectionName = "quartz";
    private const string RegistryKey = "jobs.quartz";

    public static IJalpanBuilder AddQuartz(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrEmpty(sectionName) ? DefaultSectionName : sectionName;

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<QuartzOptions>();
        builder.Services.Configure<QuartzOptions>(section);

        if (!builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        builder.Services.AddQuartz(q =>
        {
            if (options is { PostgresPersistency.Enabled: true, SqlServerPersistency.Enabled: true })
            {
                throw new QuartzConfigurationException(
                    "Both PostgresPersistency and SqlServerPersistency are enabled. Only one persistency provider can be enabled at a time.");
            }

            if (options.PostgresPersistency is { Enabled: true })
            {
                if (string.IsNullOrEmpty(options.PostgresPersistency.ConnectionString))
                {
                    throw new QuartzConfigurationException("Postgres database connection string cannot be empty.");
                }

                q.UsePersistentStore(persistentStoreOptions =>
                {
                    persistentStoreOptions.UsePostgres(postgresOptions => 
                        postgresOptions.ConnectionString = options.PostgresPersistency.ConnectionString);

                    persistentStoreOptions.UseSerializer<SystemTextJsonObjectSerializer>();
                    persistentStoreOptions.UseClustering();
                });
            }
            
            if (options.SqlServerPersistency is { Enabled: true })
            {
                if (string.IsNullOrEmpty(options.SqlServerPersistency.ConnectionString))
                {
                    throw new QuartzConfigurationException("SqlServer database connection string cannot be empty.");
                }

                q.UsePersistentStore(persistentStoreOptions =>
                {
                    persistentStoreOptions.UseSqlServer(sqlServerOptions =>
                        sqlServerOptions.ConnectionString = options.SqlServerPersistency.ConnectionString);

                    persistentStoreOptions.UseSerializer<SystemTextJsonObjectSerializer>();
                    persistentStoreOptions.UseClustering();
                });
            }

            q.AddJobs(options);
        });

        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return builder;
    }

    private static void AddJobs(this IServiceCollectionQuartzConfigurator quartzConfigurator, QuartzOptions options)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var jobTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => typeof(IJob).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .ToArray();

        foreach (var jobType in jobTypes)
        {
            quartzConfigurator.AddJobAndTrigger(jobType, options);
        }
    }

    private static void AddJobAndTrigger(this IServiceCollectionQuartzConfigurator quartzConfigurator, Type jobType,
        QuartzOptions options)
    {
        var jobKey = new JobKey(jobType.Name);
        quartzConfigurator.AddJob(jobType, jobKey);

        // Check if a Cron expression exists for this job in the configuration
        if (options.Schedules.TryGetValue(jobType.Name, out var cronSchedule))
        {
            quartzConfigurator.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{jobType.Name}-trigger")
                .WithCronSchedule(cronSchedule));
        }
        else
        {
            throw new CronScheduleException(
                $"No Quartz.NET Cron schedule found for job in configuration at {jobType.Name}");
        }
    }
}