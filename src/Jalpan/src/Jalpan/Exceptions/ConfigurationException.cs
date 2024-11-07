namespace Jalpan.Exceptions;

public class ConfigurationException(string message, string path)
    : CustomException($"Configuration error at '{path}': {message}");
