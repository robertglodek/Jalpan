namespace Jalpan.WebApi.Exceptions;

public class ErrorResponse
{
    /// <summary>
    /// A URI reference [RFC3986] that identifies the problem type.
    /// </summary>
    public string? Type { get; set; }
    /// <summary>
    /// Error identifying code
    /// </summary>
    public string Code { get; set; } = null!;
    /// <summary>
    /// Details describing the error
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// A URI reference that identifies the specific occurrence of the problem. 
    /// It may or may not yield further information if dereferenced.
    /// </summary>
    public string? Instance { get; set; }
}
