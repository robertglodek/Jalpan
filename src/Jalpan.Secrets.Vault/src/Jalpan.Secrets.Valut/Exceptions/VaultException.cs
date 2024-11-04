using Jalpan.Exceptions;

namespace Jalpan.Secrets.Valut.Exceptions;

internal sealed class VaultException(string message, Exception? innerException = null)
    : CustomException(message, innerException)
{
}