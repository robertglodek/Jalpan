using Jalpan.Auth.Distributed.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Auth.Distributed;

public static class Extensions
{
    private const string RegistryName = "auth.distributed";

    public static IJalpanBuilder AddDistributedAccessTokenValidator(this IJalpanBuilder builder)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton<IAccessTokenService, DistributedAccessTokenService>();

        return builder;
    }
}
