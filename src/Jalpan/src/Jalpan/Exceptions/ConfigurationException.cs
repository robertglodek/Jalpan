namespace Jalpan.Exceptions;

public class ConfigurationException : Exception
{
    public string PropertyName { get; }

    public ConfigurationException(string message, string propertyName)
        : base(message)
    {
        PropertyName = propertyName;
    }

    public override string ToString() => $"{base.ToString()}, PropertyName: {PropertyName}";
}
