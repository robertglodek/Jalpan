using Jalpan.Channels.Email.SendGrid.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;

namespace Jalpan.Channels.Email.SendGrid;

public static class Extensions
{
    private const string DefaultSectionName = "sendGrid";
    private static int _initialized;

    public static IServiceCollection AddSendGrid(this IServiceCollection services, IConfiguration configuration, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrEmpty(sectionName) ? DefaultSectionName : sectionName;

        if (Interlocked.Exchange(ref _initialized, 1) == 1)
        {
            return services;
        }

        var section = configuration.GetSection(sectionName);
        var options = section.BindOptions<SendGridOptions>();

        services.Configure<SendGridOptions>(section);

        services.AddSendGrid(options =>
        {
            options.ApiKey = options.ApiKey;
        });
        services.AddScoped<IEmailService, SendGridEmailService>();

        return services;
    }
}
