namespace Jalpan.Validation;

public sealed class ValidationError(string message, string code)
{
    public string Message { get; } = message;
    public string Code { get; } = code;
}