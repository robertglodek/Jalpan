namespace Jalpan.WebApi.Swagger;

public sealed class SwaggerOptions
{
    public bool Enabled { get; set; } = true;
    public bool ReDocEnabled { get; set; } = false;
    public string Name { get; set; } = "v1";
    public string Title { get; set; } = "API Documentation";
    public string Version { get; set; } = "1.0";
    public string? RoutePrefix { get; set; }
    public bool IncludeSecurity { get; set; } = false;
}
