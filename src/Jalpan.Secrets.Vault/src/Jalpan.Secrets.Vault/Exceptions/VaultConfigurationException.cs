using Jalpan.Exceptions;

namespace Jalpan.Secrets.Vault.Exceptions;

internal sealed class VaultConfigurationException(string message, Exception? innerException = null)
    : CustomException(message, innerException)
{
}