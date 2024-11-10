using Jalpan.Exceptions;

namespace Jalpan.HTTP.Exceptions;

public sealed class HttpConfigurationException(string message) : CustomException(message); 