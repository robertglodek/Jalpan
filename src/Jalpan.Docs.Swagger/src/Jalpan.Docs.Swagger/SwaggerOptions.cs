namespace Jalpan.Docs.Swagger;

public class SwaggerOptions
{
    public bool Enabled { get; set; }
    public bool ReDocEnabled { get; set; }
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string? RoutePrefix { get; set; }
    public bool IncludeSecurity { get; set; }
}
