namespace Jalpan.Secrets.Valut.Exceptions;

internal sealed class VaultAuthTypeNotSupportedException(string message, string authType) : Exception(message)
{
    public string AuthType { get; set; } = authType;

    public VaultAuthTypeNotSupportedException(string authType) : this(string.Empty, authType)
    {
    }
}