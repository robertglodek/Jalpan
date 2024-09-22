using Jalpan.Security.Encryption;
using Jalpan.Security.Hashing;
using Jalpan.Security.Signing;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.Security;

public static class Extensions
{
    public static IJalpanBuilder AddSecurity(this IJalpanBuilder builder)
    {
        builder.Services
            .AddSingleton<IEncryptor, Encryptor>()
            .AddSingleton<IHasher, Hasher>()
            .AddSingleton<ISigner, Signer>();

        return builder;
    }
}