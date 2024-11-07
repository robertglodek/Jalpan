namespace Jalpan;

public sealed class AppOptions
{
    public string Name { get; init; } = string.Empty;
    public string Service { get; init; } = string.Empty;
    public string Instance { get; init; } = string.Empty;
    public string Version { get; init; } = "v1";
}
