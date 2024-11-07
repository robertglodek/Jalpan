using Jalpan.Exceptions;

namespace Jalpan.Secrets.Vault.Exceptions;

internal sealed class VaultAuthTypeNotSupportedException(string message, string authType) : CustomException(message)
{
    public string AuthType { get; set; } = authType;

    internal VaultAuthTypeNotSupportedException(string authType) : this(string.Empty, authType)
    {
    }
}