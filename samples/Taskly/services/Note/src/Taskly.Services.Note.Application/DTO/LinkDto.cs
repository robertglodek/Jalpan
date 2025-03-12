namespace Taskly.Services.Note.Application.DTO;

public sealed class LinkDto
{
    public string Url { get; set; } = null!;
    public string? Name { get; set; }
}