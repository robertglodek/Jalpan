namespace Jalpan.Secrets.Valut.Exceptions;

internal sealed class VaultException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
}