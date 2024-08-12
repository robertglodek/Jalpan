namespace Jalpan.WebApi.Exceptions;

public class ValidationErrorResponse : ErrorResponse
{
    /// <summary>
    /// Validation errors
    /// </summary>
    public IDictionary<string, string[]> Errors { get; } = null!;
}
