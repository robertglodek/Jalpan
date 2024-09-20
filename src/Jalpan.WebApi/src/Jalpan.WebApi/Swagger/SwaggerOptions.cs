namespace Jalpan.WebApi.Swagger;

public class SwaggerOptions
{
    public bool Enabled { get; set; }
    public bool ReDocEnabled { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string? RoutePrefix { get; set; }
    public bool IncludeSecurity { get; set; }
}
