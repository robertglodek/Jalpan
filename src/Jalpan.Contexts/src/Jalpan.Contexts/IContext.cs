namespace Jalpan.Contexts;

public interface IContext
{
    string ActivityId { get; }
    string? UserId { get; }
    string? MessageId { get; }
    string? Role { get; }
    IDictionary<string, string[]>? Claims { get; }
}