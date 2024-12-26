using Jalpan.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Taskly.Services.Identity.Application;

public static class Extensions
{
    private const string RefreshTokenSectionName = "jwt";

    public static IJalpanBuilder AddApplication(this IJalpanBuilder builder)
    {
        builder.AddSecurity();
        builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection(RefreshTokenSectionName));
        return builder;
    }
}