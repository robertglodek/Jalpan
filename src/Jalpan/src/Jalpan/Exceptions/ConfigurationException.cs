namespace Jalpan.Exceptions;

public class ConfigurationException(string message, string propertyName) : CustomException(message)
{
    public string PropertyName { get; } = propertyName;

    public override string ToString() => $"{base.ToString()}, PropertyName: {PropertyName}";
}
